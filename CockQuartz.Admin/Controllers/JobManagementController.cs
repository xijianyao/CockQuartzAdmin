﻿using System.Web.Mvc;
using CockQuartz.Interface;
using CockQuartz.Model;

namespace CockQuartzAdmin.Controllers
{
    public class JobManagementController : Controller
    {
        private readonly IJobService _jobService;

        public JobManagementController(IJobService jobService)
        {
            _jobService = jobService;
        }

        [HttpGet]
        public ActionResult JobList(string groupName = "自驾官网")
        {
            var result = _jobService.GetJobList(1, groupName);
            var instances = _jobService.GetQuartzInstances();
            ViewBag.Instances = instances;
            ViewBag.GroupName = groupName;
            return View(result);
        }
        [HttpPost]
        public ViewResult JobList(int pageIndex = 1, string groupName = "")
        {
            var result = _jobService.GetJobList(pageIndex, groupName);
            ViewBag.GroupName = groupName;
            return View("Partial/_JobList",result);
        }

        [HttpGet]
        public ActionResult JobLogList(int jobId)
        {
            var result = _jobService.GetJobLogList(jobId);
            return View(result);
        }

        [HttpPost]
        public JsonResult CreateJob(JobDetail job)
        {
            _jobService.CreateJob(job);
            return Json(true);
        }

        [HttpPost]
        public JsonResult RunJob(int id)
        {
            _jobService.RunJob(id);
            return Json(new { success = true, message = "成功" });
        }

        [HttpPost]
        public JsonResult DeleteJob(int id)
        {
            _jobService.DeleteJob(id);
            return Json(new { success = true, message = "成功" });
        }

        [HttpPost]
        public JsonResult PauseJob(int id)
        {
            _jobService.PauseJob(id);
            return Json(new { success = true, message = "成功" });
        }

        [HttpPost]
        public JsonResult StartJob(int id)
        {
            _jobService.StartJob(id);
            return Json(new { success = true, message = "成功" });
        }

        [HttpPost]
        public JsonResult ResumeJob(int id)
        {
            _jobService.ResumeJob(id);
            return Json(new { success = true, message = "成功" });
        }

        [HttpPost]
        public JsonResult ModifyJobCron(int id, string cron)
        {
            _jobService.ModifyJobCron(id, cron);
            return Json(new { success = true, message = "成功" });
        }

        [HttpPost]
        public JsonResult ModifyExceptionEmail(int id, string exceptionEmail)
        {
            _jobService.ModifyExceptionEmail(id, exceptionEmail);
            return Json(new { success = true, message = "成功" });
        }

        [HttpPost]
        public JsonResult ModifyRequestUrl(int id, string requestUrl)
        {
            _jobService.ModifyRequestUrl(id, requestUrl);
            return Json(new { success = true, message = "成功" });
        }
    }
}