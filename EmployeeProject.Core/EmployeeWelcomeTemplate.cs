
using System.Net;


namespace EmployeeProject.Core
{
    public class EmployeeWelcomeTemplate
    {
        public static string Generate(
            int employeeId,
            string employeeName,
            string email,
            string temporaryPassword,
            string loginUrl)
        {
            return $"""
                <!DOCTYPE html>
                <html>
                <body style="font-family: Arial, sans-serif; color: #333;">

                    <h2>Welcome to Employee Management System</h2>

                    <p>Hello {WebUtility.HtmlEncode(employeeName)},</p>

                    <p>Your employee account has been created successfully.</p>

                    <p><strong>Employee ID:</strong> {employeeId}</p>
                    <p><strong>Email:</strong> {WebUtility.HtmlEncode(email)}</p>
                    <p><strong>Temporary Password:</strong>
                    {WebUtility.HtmlEncode(temporaryPassword)}</p>

                    <p>Please log in and change your password immediately.</p>

                    <div style="text-align: center; margin: 25px 0;">
                        <a href="{WebUtility.HtmlEncode(loginUrl)}"
                           style="background-color: #0d6efd;
                                  color: white;
                                  padding: 12px 25px;
                                  text-decoration: none;
                                  border-radius: 5px;
                                  display: inline-block;">
                            Login to Employee Portal
                        </a>
                    </div>

                    <p>Regards,<br />
                    Employee Management Team</p>

                </body>
                </html>
                """;
        }
    }
}
