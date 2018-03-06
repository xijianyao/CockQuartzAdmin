using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CockQuartz.Interface;

namespace CockQuartzAdmin.Controllers
{
    public class JobManagementController : Controller
    {
        private readonly ICustomerJobInfoService _customerJobInfoService;

        public JobManagementController(ICustomerJobInfoService customerJobInfoService)
        {
            _customerJobInfoService = customerJobInfoService;
        }

        // GET: JobManagement
        public ActionResult Index()
        {
            _customerJobInfoService.AddCustomerJobInfo("", "", "", "", "", "", "", "");
            return View();
        }
    }
}