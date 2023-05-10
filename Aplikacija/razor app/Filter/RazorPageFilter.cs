using System.Globalization;
using backend.Enums;
using backend.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
namespace backend.Filter
{
    public class RazorPageFilter : IPageFilter
    {
        private readonly ISessionDataService _sessionDataService;

        public RazorPageFilter(ISessionDataService sessionDataService)
        {
            _sessionDataService = sessionDataService;
        }

        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
            var user = _sessionDataService.GetSessionData(context.HttpContext.Session);
            var pagename = context.HttpContext.Request.Path.Value;
            var currentPage = user.CachingData.CurrentPage;
            pagename = pagename.ToLower();
            
            if (currentPage.Contains("/administratorpage") && !pagename.Contains("/administratorpage"))
            {
                user.CachingData.RemoveAll();
            }
            if (currentPage.Contains("/trenerpage") && !pagename.Contains("/trenerpage"))
            {
                user.CachingData.RemoveAllTrenerPage();
            }
            if (currentPage.Contains("/prodavnica") && !pagename.Contains("/prodavnica"))
            {
                user.CachingData.RemoveAll();
            }
            if (currentPage.Contains("/proizvod") && !pagename.Contains("/proizvod"))
            {
                user.CachingData.proizvodID= -99;
            }
            if (currentPage.Contains("/viewprofilepage") && !pagename.Contains("/viewprofilepage"))
            {
                user.CachingData.proizvodID= -99;
                user.CachingData.CPT = 0;
            }

            if ((pagename.Contains("/proizvod") || pagename.Contains("/viewprofilepage")) &&
                user.CachingData.proizvodID == -99)
            {
                if (pagename.Contains("/proizvod"))
                    context.HttpContext.Response.Redirect("/prodavnica?culture=" + CultureInfo.CurrentCulture);
                else 
                    context.HttpContext.Response.Redirect("/index?culture=" + CultureInfo.CurrentCulture);
            }

            if (pagename == "/")
            {
                if (user.TipKorisnika == UserCategory.NotLogged)
                    context.HttpContext.Response.Redirect("/index?culture="+ CultureInfo.CurrentCulture);
            }

            if (user.TipKorisnika == UserCategory.NotLogged && (pagename.Contains("/administratorpage") ||
                                                            pagename.Contains("/korisnik") || 
                                                            pagename.Contains("/trenerpage") ||
                                                            pagename.Contains("/korisnikpage") ||
                                                            pagename.Contains("/korisnikedit") ||
                                                            pagename.Contains("/viewprofilepage")))
                context.HttpContext.Response.Redirect("/prijava?culture="+ CultureInfo.CurrentCulture);

            if (user.TipKorisnika == UserCategory.DefaultUser && (pagename.Contains("/administratorpage") ||
                                                             pagename.Contains("/trenerpage")))
                context.HttpContext.Response.Redirect("/index?culture="+ CultureInfo.CurrentCulture);

            if ((user.TipKorisnika == UserCategory.PersonalTrainer) &&
                (pagename.Contains("/administratorpage") || pagename.Contains("/korisnikpage")))
                context.HttpContext.Response.Redirect("/korisnik?culture="+ CultureInfo.CurrentCulture);
            
            if (user.TipKorisnika != UserCategory.NotLogged && (pagename.Contains("/registracija") || pagename.Contains("/prijava")))
                context.HttpContext.Response.Redirect("/korisnik?culture="+ CultureInfo.CurrentCulture);
            user.CachingData.CurrentPage = context.ActionDescriptor.DisplayName.ToLower();
            _sessionDataService.SetSessionData(context.HttpContext.Session,user);
        }

        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
        }

        public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
        }
    }
}