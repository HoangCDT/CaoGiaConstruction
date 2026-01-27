using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;

namespace CaoGiaConstruction.WebClient.Dtos
{
    public class ProcessStepDto
    {
        public List<ProcessStepVM> Steps { get; set; }
        public int TotalSteps { get; set; }
        public SettingVM Setting { get; set; }
    }
}
