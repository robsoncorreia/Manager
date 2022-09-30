using CommunityToolkit.Mvvm.DependencyInjection;
using FC.Domain.Model.Project;
using FC.Domain.Service;

using System;
using System.Globalization;
using System.Windows.Controls;

namespace FC.Domain.Validator
{
    public class ModuleIdValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                return value is null
                    ? new ValidationResult(false, Properties.Resources.Invalid_ID)
                    : string.IsNullOrEmpty(value.ToString())
                    ? new ValidationResult(false, Properties.Resources.Invalid_ID)
                    : !int.TryParse(value.ToString(), out int moduleId)
                    ? new ValidationResult(false, Properties.Resources.Invalid_ID)
                    : moduleId is 0 or > 99
                    ? new ValidationResult(false, Properties.Resources.Invalid_ID)
                    : Ioc.Default.GetService<IProjectService>().SelectedProject is not ProjectModel project
                    ? new ValidationResult(false, Properties.Resources.Invalid_ID)
                    : 1 == moduleId
                    ? new ValidationResult(false, Properties.Resources.Id_must_be_different_from_gateway_Id_)
                    : project.SelectedGateway.ModuleId == moduleId
                    ? new ValidationResult(false, Properties.Resources.Id_must_be_different_from_gateway_Id_)
                    : new ValidationResult(true, null);
            }
            catch (Exception)
            {
                return new ValidationResult(false, Properties.Resources.Select_a_combobox_item);
            }
        }
    }
}