using Microsoft.AspNetCore.Mvc;
using CaoGiaConstruction.WebClient.Dtos;
using CaoGiaConstruction.WebClient.Services;
using CaoGiaConstruction.WebClient.AutoMapper.ViewModels;
using AutoMapper;

namespace CaoGiaConstruction.WebClient.Controllers.ViewComponents
{
    public class vcProcessStep : ViewComponent
    {
        private readonly IProcessStepService _processStepService;
        private readonly IMapper _mapper;

        public vcProcessStep(IProcessStepService processStepService, IMapper mapper)
        {
            _processStepService = processStepService;
            _mapper = mapper;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var processSteps = await _processStepService.GetActiveProcessStepsAsync();
            
            var stepsVM = _mapper.Map<List<ProcessStepVM>>(processSteps);
            var totalSteps = stepsVM.Count;

            var result = new ProcessStepDto
            {
                Steps = stepsVM,
                TotalSteps = totalSteps,
                Setting = ViewBag.Setting as SettingVM
            };

            return View(result);
        }
    }
}
