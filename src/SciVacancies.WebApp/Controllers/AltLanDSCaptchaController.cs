﻿using System;
using System.Drawing.Imaging;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.OptionsModel;
using SciVacancies.Captcha;

namespace SciVacancies.WebApp.Controllers
{

    public class AltLanDSCaptchaController : Controller
    {
        private readonly IOptions<CaptchaSettings> _captchaSettings;

        public AltLanDSCaptchaController(IOptions<CaptchaSettings> captchaSettings)
        {
            _captchaSettings = captchaSettings;
        }

        /// <summary>
        /// Сгенерировать изображение с кодом
        /// </summary>
        /// <returns></returns>
        [ResponseCache(NoStore = false, Duration = 0)]
        public FileContentResult Fetch()
        {
            if (CaptchaConfiguration.Referer.Length > 0
                 //&& (Context.Request.UrlReferrer == null ||
                 // Context.Request.UrlReferrer.Host.IndexOf(CaptchaConfiguration.Referer, StringComparison.Ordinal) < 0)
                 )
            {
                //NotFound(context);
                //return;
                throw new Exception();
            }

            var text = CaptchaConfiguration.DefaultTextGeneratorFactory().GenerateText();
            var ci = new CaptchaImage(text);

            int imageWidth, imageHeight;
            if (int.TryParse(HttpContext.Request.Query["w"], out imageWidth))
            {
                ci.Width = imageWidth;
            }
            if (int.TryParse(HttpContext.Request.Query["h"], out imageHeight))
            {
                ci.Height = imageHeight;
            }

            bool renew;
            bool.TryParse(HttpContext.Request.Query["r"], out renew);

            var captchaContext = new CaptchaContext(HttpContext);

            var key = renew ? captchaContext.GetCaptchaKey() : Guid.NewGuid().ToString();

            var iCaptchaStore = CaptchaConfiguration.DefaultStoreFactory(HttpContext);
            iCaptchaStore.PutCaptcha(key, text);

            captchaContext.WriteCaptchaCookie(key, _captchaSettings.Value.CaptchaDurationSeconds);

            FileContentResult result;
            using (var b = ci.RenderImage())
            using (var memStream = new System.IO.MemoryStream())
            {
                b.Save(memStream, ImageFormat.Gif);
                result = File(memStream.GetBuffer(), "image/jpeg");
            }
            return result;

            //using (var b = ci.RenderImage())
            //{
            //    b.Save(Context.Response.OutputStream, ImageFormat.Gif);
            //}

            //captchaContext.WriteCaptchaCookie(key, _captchaSettings.Options.CaptchaDurationSeconds);
            //Context.Response.ContentType = "image/gif";
            //Context.Response.StatusCode = 200;
            ////Context.Response.StatusDescription = "OK";
            ////Context.ApplicationInstance.CompleteRequest();
            //return null;
        }

        /// <summary>
        /// Проверка правильности введенного кода
        /// </summary>
        /// <param name="captchaText"></param>
        /// <returns></returns>
        [ResponseCache(NoStore = false, Duration = 0)]
        [HttpPost]
        public IActionResult IsValid(string captchaText)
        {
            var verifier = new CaptchaVerifier(CaptchaConfiguration.DefaultStoreFactory(HttpContext), HttpContext);
            bool boolResult = false;
            try
            {
                boolResult = verifier.IsValid(captchaText);
            }
            catch (Exception e)
            {
                throw e;
            }

            return Json(boolResult);
        }

    }
}