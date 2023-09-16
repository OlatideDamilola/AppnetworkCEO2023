using AppnetworkCEO2023.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using static HelperModule.net.UtilityClass;

namespace AppnetworkCEO2023.Controllers {
    public  class DashboardLayoutVmodel {
        public string FullName { get; set;}
        public string RefCode { get; set;}
        public string RegDate { get; set;}
        public string PicName { get; set;}
    }

    public  class BonusHistoryVmodel {
        public string Phone { get; set;}
        public string RegDate { get; set;}
    }

    public class DashboardController : Controller {

        private readonly AppnetworkCeodbContext _context;
        public DashboardController(AppnetworkCeodbContext context) {
            _context = context;
        }


        public IActionResult Index() {

            try {
                if (HttpContext.Session.GetString("logId") != null) {
                    if (HttpContext.Session.GetString("logId").Contains(RegTypeStruct.Ceomembership)) {
                        RedirectToAction("Ceomember");
                        //logId = int.Parse(HttpContext.Session.GetString("logId").Split("/")[0]);
                    } else if (HttpContext.Session.GetString("logId").Contains(RegTypeStruct.Shareholder)) RedirectToAction("Shareholder");
                }
            } catch {
                HttpContext.Session.SetString("logId", String.Empty);
                RedirectToAction("index", "home"); RedirectToRoute("home");
            }
            HttpContext.Session.SetString("logId", String.Empty);
            RedirectToRoute("home");

        }

        //bonusTot = ret.SubscriberReferalBonuses.Sum(sX=>sX.Amount)
        public async Task<IActionResult> Ceomember() {
            int logId;
            dynamic Vdata = new ExpandoObject();
            try {
                logId = int.Parse(HttpContext.Session.GetString("logId").Split("/")[0]);
                using (var Db = _context) {
                    var dbRet =await Db.SubscriberRegisters.AsNoTracking()
                         .Include(sRefBonus => sRefBonus.SubscriberReferalBonuses)
                         .Select(ret => new { ret.Id, ret.Surname, ret.Firstname, ret.PictureName, ret.Phone, ret.RegisteredDate,
                             bonusOwnersIdList= ret.SubscriberReferalBonuses.Select(x => x.OwnerId).ToList(),
                             totBonus=ret.SubscriberReferalBonuses.Select(tB=>tB.Amount).Sum()})
                         .FirstOrDefaultAsync();

                    if (dbRet != null) {
                        DashboardLayoutVmodel Lmodel = new();
                      //List< BonusHistoryVmodel> BhistoryList = new();
                        Lmodel.FullName = dbRet.Surname + " " + dbRet.Firstname;
                        Lmodel.PicName = dbRet.PictureName;
                        Lmodel.RefCode = dbRet.Id.ToString();
                        Lmodel.RegDate = dbRet.RegisteredDate.ToShortDateString();

                      //  BonusHistoryVmodel Bhistory;
                        var Bhistory = await Db.SubscriberRegisters.AsNoTracking()
                            .Where(x =>dbRet.bonusOwnersIdList.Contains(x.Id))
                            .Select(xp=>new { xp.Phone,regDate= xp.RegisteredDate.ToShortDateString() })
                            .ToListAsync();
                        Vdata.layModel=Lmodel;
                        Vdata.boModel = Bhistory;
                        Vdata.totBonus = dbRet.totBonus;
                    }
                }
            } catch {

            }

            return View();
        }


        public IActionResult Shareholder() {
            return View();
        }

    }
}
}
