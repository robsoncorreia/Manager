using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using System.Text.RegularExpressions;

namespace QRCodeGenerator.Util
{
    public static class ExtensionMethods
    {
        public static bool IsEmailValid(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                static string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    IdnMapping idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public static string GetEnumDescription(this Enum enumObj)
        {
            if (enumObj is null)
            {
                return string.Empty;
            }

            if (!(enumObj.GetType().GetField(enumObj.ToString()) is FieldInfo fieldInfo))
            {
                return string.Empty;
            }

            object[] attribArray = fieldInfo.GetCustomAttributes(false);

            if (attribArray.Length == 0)
            {
                return enumObj.ToString();
            }
            else
            {
                DescriptionAttribute attrib = attribArray[0] as DescriptionAttribute;
                return attrib.Description;
            }
        }

        public static void Save(this MailMessage message, string filename, bool addUnsentHeader = true)
        {
            using FileStream filestream = File.Open(filename, FileMode.Create);

            if (addUnsentHeader)
            {
                BinaryWriter binaryWriter = new BinaryWriter(filestream);
                //Write the Unsent header to the file so the mail client knows this mail must be presented in "New message" mode
                binaryWriter.Write(System.Text.Encoding.UTF8.GetBytes("X-Unsent: 1" + Environment.NewLine));
            }

            Assembly assembly = typeof(SmtpClient).Assembly;
            Type mailWriterType = assembly.GetType("System.Net.Mail.MailWriter");

            // Get reflection info for MailWriter contructor
            ConstructorInfo mailWriterContructor = mailWriterType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(Stream) }, null);

            // Construct MailWriter object with our FileStream
            object mailWriter = mailWriterContructor.Invoke(new object[] { filestream });

            // Get reflection info for Send() method on MailMessage
            MethodInfo sendMethod = typeof(MailMessage).GetMethod("Send", BindingFlags.Instance | BindingFlags.NonPublic);

            _ = sendMethod.Invoke(message, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { mailWriter, true, true }, null);

            // Finally get reflection info for Close() method on our MailWriter
            MethodInfo closeMethod = mailWriter.GetType().GetMethod("Close", BindingFlags.Instance | BindingFlags.NonPublic);

            // Call close method
            _ = closeMethod.Invoke(mailWriter, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { }, null);
        }

        public static bool ValidateEmail(this string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return false;
            }

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");

            return regex.Match(email).Success;
        }
    }
}