using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using SoftUni.Data;
using SoftUni.Models;
using System.Globalization;
using System.Text;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            SoftUniContext context = new SoftUniContext();

            var output = string .Empty;
            //03.
            //output = GetEmployeesFullInformation(context);

            //04.
            //output = GetEmployeesWithSalaryOver50000(context);

            //05.
            //output = GetEmployeesFromResearchAndDevelopment(context);

            //06.
            //output = AddNewAddressToEmployee(context);

            //07.
            //output = GetEmployeesInPeriod(context);

            //08.
            //output = GetAddressesByTown(context);

            //09.
            //output = GetEmployee147(context);

            //10.
            //output = GetDepartmentsWithMoreThan5Employees(context);

            //11.
            //output = GetLatestProjects(context);

            //12.
            //output = IncreaseSalaries(context);

            //13.
            //output = GetEmployeesByFirstNameStartingWithSa(context);

            //14.
            //output = DeleteProjectById(context);

            //15.
            output = RemoveTown(context);

            Console.WriteLine(output);
        }

        //3. Employees Full Information
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {

            var employees = context.Employees
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.MiddleName,
                    x.JobTitle,
                    x.Salary
                }).ToList();

            var result = string.Join(Environment.NewLine,
                employees.Select(e => $"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:f2}"));
            return result;
        }
        //4. Employees with Salary Over 50 000
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employess = context.Employees
                .Select(x => new
                {
                    x.FirstName,
                    x.Salary

                })
                .Where(x => x.Salary > 50000)
                .OrderBy(x => x.FirstName);
            var result = string.Join(Environment.NewLine, employess.Select(e => $"{e.FirstName} - {e.Salary:f2}"));
            return result;
        }
        //5. Employees from Research and Development
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {

            var employess = context.Employees.Select(e => new
            {
                e.FirstName,
                e.LastName,
                e.Department.Name,
                e.Salary
            })
            .Where(x => x.Name == "Research and Development")
            .OrderBy(x => x.Salary)
            .ThenByDescending(x => x.FirstName)
            .ToList();

            var result = string.Join(Environment.NewLine,
                employess.Select(e => $"{e.FirstName} {e.LastName} from {e.Name} - ${e.Salary:f2}"));
            return result;
        }
        //6. Adding a New Address and Updating Employee
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            Address address = new Address()
            {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            var employee = context.Employees.FirstOrDefault(x => x.LastName == "Nakov");
            employee.Address = address;
            context.SaveChanges();

            var employess = context.Employees.Select(x => new
            {
                x.AddressId,
                x.Address.AddressText

            }).OrderByDescending(x => x.AddressId)
            .Take(10)
            .ToList();
            var result = string.Join(Environment.NewLine, employess.Select(x => $"{x.AddressText}"));
            return result;
        }
        //7. Employees and Projects
        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var employees = context.Employees.Select(x => new
            {
                x.FirstName,
                x.LastName,
                ManagerFirstName = x.Manager.FirstName,
                ManagerLastName = x.Manager.LastName,
                Projects = x.EmployeesProjects.Where(x => x.Project.StartDate.Year >= 2001 && x.Project.StartDate.Year <= 2003)

                    .Select(ep => new
                    {
                        ProjectName = ep.Project.Name,
                        StartDate = ep.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt"),
                        EndDate = ep.Project.EndDate != null
                            ? ep.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt")
                            : "not finished"
                    })
            }).Take(10)
            .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.ManagerFirstName} {e.ManagerLastName}");

                if (e.Projects.Any())
                {
                    sb.AppendLine(String.Join(Environment.NewLine, e.Projects
                        .Select(p => $"--{p.ProjectName} - {p.StartDate} - {p.EndDate}")));
                }
            }

            return sb.ToString().TrimEnd();
        }
        //8. Addresses by Town
        public static string GetAddressesByTown(SoftUniContext context)
        {
            var addresses = context.Addresses
                .OrderByDescending(x => x.Employees.Count)
                .ThenBy(x => x.Town.Name)
                .ThenBy(x => x.AddressText)
                .Take(10)
                .Select(x => $"{x.AddressText}, {x.Town.Name} - {x.Employees.Count} employees")
                .ToList();




            return string.Join(Environment.NewLine, addresses);
        }
        //09. Employee 147
        public static string GetEmployee147(SoftUniContext context)
        {
            var employee = context.Employees.Where(x => x.EmployeeId == 147)
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    x.JobTitle,
                    Projects = x.EmployeesProjects.Select(x => new { x.Project.Name }).OrderBy(x => x.Name).ToList()
                })
                .FirstOrDefault();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");
            sb.Append(string.Join(Environment.NewLine, employee.Projects.Select(p => p.Name)));




            return sb.ToString().TrimEnd();
        }
        //10. Departments with More Than 5 Employees
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var departments = context.Departments
              .Where(d => d.Employees.Count > 5)
              .OrderBy(d => d.Employees.Count)
              .ThenBy(d => d.Name)
              .Select(d => new
              {
                  DepartmentName = d.Name,
                  ManagerName = d.Manager.FirstName + " " + d.Manager.LastName,
                  Employees = d.Employees
                      .OrderBy(e => e.FirstName)
                      .ThenBy(e => e.LastName)
                      .Select(e => new
                      {
                          EmployeeData = $"{e.FirstName} {e.LastName} - {e.JobTitle}"
                      })
                      .ToArray()
              });


            StringBuilder sb = new StringBuilder();
            foreach (var d in departments)
            {
                sb.AppendLine($"{d.DepartmentName} - {d.ManagerName}");
                sb.Append(string.Join(Environment.NewLine,
                    d.Employees.Select(x => x.EmployeeData)));
            }


            return sb.ToString().TrimEnd();
        }

        //11.Find Latest 10 Projects
        public static string GetLatestProjects(SoftUniContext context)
        {
            var projectsInfo = context.Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .OrderBy(p => p.Name)
                .Select(p => new
                {
                    p.Name,
                    p.Description,
                    StartDate = p.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture),
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();
            foreach (var p in projectsInfo)
            {
                sb.AppendLine(p.Name);
                sb.AppendLine(p.Description);
                sb.AppendLine(p.StartDate);
            }

            return sb.ToString().TrimEnd();
        }

        //12.Increase Salaries
        public static string IncreaseSalaries(SoftUniContext context)
        {
            decimal salaryModifier = 1.12m;
            string[] departmentNames = new string[] { "Engineering", "Tool Design", "Marketing", "Information Services" };

            var employeesForSalaryIncrease = context.Employees
                .Where(e => departmentNames.Contains(e.Department.Name))
                .ToArray();

            foreach (var e in employeesForSalaryIncrease)
            {
                e.Salary *= salaryModifier;
            }

            context.SaveChanges();

            string[] emplyeesInfoText = context.Employees
                .Where(e => departmentNames.Contains(e.Department.Name))
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .Select(e => $"{e.FirstName} {e.LastName} (${e.Salary:f2})")
                .ToArray();

            return string.Join(Environment.NewLine, emplyeesInfoText);
        }

        //13.Find Employees by First Name Starting With Sa
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            string[] employeesInfoText = context.Employees
                .Where(e => e.FirstName.Substring(0, 2).ToLower() == "sa")
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .Select(e => $"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:f2})")
                .ToArray();

            return string.Join(Environment.NewLine, employeesInfoText);
        }

        //14.Delete Project by Id
        public static string DeleteProjectById(SoftUniContext context)
        {
            var employeesProjectsToDelete = context.EmployeesProjects.Where(ep => ep.ProjectId == 2);
            context.EmployeesProjects.RemoveRange(employeesProjectsToDelete);

            var projectToDelete = context.Projects.Where(p => p.ProjectId == 2);
            context.Projects.RemoveRange(projectToDelete);

            context.SaveChanges();

            string[] projectsNames = context.Projects
                .Take(10)
                .Select(p => p.Name)
                .ToArray();

            return string.Join(Environment.NewLine, projectsNames);
        }

        //15.Remove Town
        public static string RemoveTown(SoftUniContext context)
        {
            Town townToDelete = context.Towns
                    .Where(t => t.Name == "Seattle")
                    .FirstOrDefault();

            Address[] addressesToDelete = context.Addresses
                .Where(a => a.TownId == townToDelete.TownId)
                .ToArray();

            Employee[] employeesToRemoveAddressFrom = context.Employees
                .Where(e => addressesToDelete
                .Contains(e.Address))
                .ToArray();

            foreach (Employee e in employeesToRemoveAddressFrom)
            {
                e.AddressId = null;
            }

            context.Addresses.RemoveRange(addressesToDelete);
            context.Towns.Remove(townToDelete);
            context.SaveChanges();

            return $"{addressesToDelete.Count()} addresses in Seattle were deleted";
        }
    }
}