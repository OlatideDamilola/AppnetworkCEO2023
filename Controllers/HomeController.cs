using AppnetworkCEO2023.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using HelperModule.net;
using static HelperModule.net.UtilityClass;
using PayStack.Net;

namespace AppnetworkCEO2023.Controllers {
    public class HomeController : Controller {
        private readonly AppnetworkCeodbContext _context;
        private static readonly PayStackApi _api = new("sk_test_4d565d7c4e00fc223282a2d828b793d4736e6768");
        public HomeController(AppnetworkCeodbContext context) {
            _context = context;
        }

        //public async Task<SubscriberRegister?> GetRefById(int id) {
        //    try {
        //        var rVal = await _context.SubscriberRegisters.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        //        return rVal;
        //    } catch { return null; }
        //}

        public async Task<bool> VerifyLog(string email, string pword) {
            try {
                using (var Db = _context) {
                    var retVal = await Db.SubscriberRegisters.AsNoTracking().Where(x => x.Email.ToLower() == email && x.Password == pword).Select(subDetial => subDetial.Id).AnyAsync();
                    if (retVal) return true;
                }
            } catch {
                return false;
            }
            return false;
        }

        public async Task<string> VerifyReferal(string refCode) {
            try {
                if (!string.IsNullOrEmpty(refCode)) {
                    var retDb = await new DbAccessClasses(_context).GetRefById(int.Parse(refCode));
                    if (retDb != null) return (retDb.Surname + " " + retDb.Firstname).ToUpper();
                }
            } catch {
                return string.Empty;
            }
            return string.Empty;
        }

        public async Task<bool> PaymentVerificationCallBack(string reference) {

            if (!string.IsNullOrEmpty(reference)) {
                try {
                    string dRef = new UtilityClass().CodeString(reference, false);
                    var response = _api.Transactions.Verify(reference);
                    if (response != null) {
                        int retAmnt = response.Data.Amount / 100;
                        if (response.Status && retAmnt == int.Parse(dRef.Split('.')[2])) {
                            using (var Db = _context) {
                                try {
                                    int refSubId = int.Parse(dRef.Split('.')[0]);
                                    var retDb = await Db.SubscriberRegisters
                                        .Where(x => x.Id == refSubId)
                                        .Include(cPay => cPay.CeoPayment)
                                        .Include(sPay => sPay.ShareholdersPayment)
                                        /// .Select(retX => new { retX.ShareholdersPayment, retX.CeoPayment,})
                                        .FirstOrDefaultAsync();
                                    if (retDb != null) {
                                        if (dRef.Split('.')[3] == RegTypeStruct.Ceomembership && (retDb.CeoPayment != null)) {
                                            retDb.CeomemberIsPaid = true;
                                            retDb.CeoPayment.Isconfirmed = true;
                                        } else if (dRef.Split('.')[3] == RegTypeStruct.Shareholder && (retDb.ShareholdersPayment != null)) {
                                            retDb.ShareholderIsPaid = true;
                                            retDb.ShareholdersPayment.Isconfirmed = true;
                                        }
                                        SubscriberReferalBonuse sBonus = new();
                                        sBonus.OwnerId = int.Parse(retDb.ReferralCode);
                                        sBonus.Amount = AllConstants.CeomembershipAmount;
                                        sBonus.BonusFrom = retDb.Id;
                                        sBonus.BonusDate = DateTime.Now;

                                        await Db.SubscriberReferalBonuses.AddAsync(sBonus);
                                        await Db.SaveChangesAsync();

                                    }
                                } catch {
                                    return false;
                                }
                            }
                        }
                    }
                } catch {
                    return false;
                }
            }
            return true;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Account(SubscriberAccount AcctReg, string RegType) {
            Tristate canSave = Tristate.isNo;
            try {
                if (HttpContext.Session.GetInt32("regId") != null) {
                    using (var Db = _context) {
                        var retDat = await Db.SubscriberRegisters.AsNoTracking()
                            .Where(x => x.Id == HttpContext.Session.GetInt32("regId"))
                            .Include(ceoMent => ceoMent.CeoMentor)
                            .Include(shNok => shNok.SharedHolderNok)
                            .Include(subAcct => subAcct.SubscriberAccount)
                            .Select(sc => new { sc.Id, sc.Surname, sc.Firstname, sc.Email, sc.CeomemberIsPaid, sc.CeoMentor, sc.ShareholderIsPaid, sc.SubscriberAccount, sc.SharedHolderNok })
                            .FirstOrDefaultAsync();
                        if (retDat?.CeoMentor != null || retDat?.SharedHolderNok != null) {
                            if (retDat?.SubscriberAccount == null) {
                                AcctReg.Id = retDat.Id;
                                await Db.SubscriberAccounts.AddAsync(AcctReg);
                                canSave = Tristate.isYes;
                            } else {
                                AcctReg.Id = retDat.Id;
                                Db.SubscriberAccounts.Update(AcctReg);
                                canSave = Tristate.isBoth;
                            }
                            if (canSave != Tristate.isNo) {
                                await Db.SaveChangesAsync();
                                var DbRet = await Db.SubscriberRegisters.AsNoTracking()
                                       .Where(c => c.Id == AcctReg.Id)
                                       .Include(CeoPayment => CeoPayment.CeoPayment)
                                       .Include(SharePayment => SharePayment.ShareholdersPayment)
                                       .Select(x => new {
                                           x.CeoPayment,
                                           x.ShareholdersPayment,
                                           x.ReferralCode,
                                           x.CeomemberIsPaid,
                                           x.ShareholderIsPaid
                                       })
                                       .FirstOrDefaultAsync();

                                Tristate canProceed = Tristate.isNo;
                                if (DbRet != null) {
                                    if (RegType == RegTypeStruct.Ceomembership) {
                                        if (DbRet.CeoPayment == null) canProceed = Tristate.isYes;
                                        else canProceed = Tristate.isBoth;
                                    } else {
                                        if (DbRet.ShareholdersPayment == null) canProceed = Tristate.isYes;
                                        else canProceed = Tristate.isBoth;
                                    }
                                    //ownId.transfId.Amnt.ceo/shd
                                    if (canProceed != Tristate.isNo) {
                                        var paymentRefCode = new UtilityClass().CodeString(AcctReg.Id.ToString() + "."
                                            + new UtilityClass().GenerateUID() + "." + (RegType == RegTypeStruct.Ceomembership
                                            ? AllConstants.CeomembershipAmount : AllConstants.ShareholderAmount) + "."
                                            + (RegType == RegTypeStruct.Ceomembership ? RegTypeStruct.Ceomembership : RegTypeStruct.Shareholder), true);
                                        var tResponse = new PayStackExtendedClass().InitializePaymentRequest(
                                            (RegType == RegTypeStruct.Ceomembership ? AllConstants.CeomembershipAmount
                                            : AllConstants.ShareholderAmount), retDat.Email, paymentRefCode, retDat.Surname + " "
                                            + retDat.Firstname, "Payment for " + (RegType == RegTypeStruct.Ceomembership
                                            ? RegTypeStruct.Ceomembership : RegTypeStruct.Shareholder), "");
                                        if (tResponse != null && tResponse.Status) {

                                            if (RegType == RegTypeStruct.Ceomembership) {
                                                CeoPayment recDat = new();
                                                recDat.Id = AcctReg.Id;
                                                recDat.PaymentDate = DateTime.Now;
                                                recDat.PaymentJson = tResponse.RawJson;
                                                recDat.Amount = AllConstants.CeomembershipAmount;
                                                recDat.Isconfirmed = false;
                                                recDat.TransacRefId = paymentRefCode;
                                                await Db.CeoPayments.AddAsync(recDat);
                                            } else if (RegType == RegTypeStruct.Shareholder) {
                                                ShareholdersPayment recDat = new();
                                                recDat.Id = AcctReg.Id;
                                                recDat.PaymentDate = DateTime.Now;
                                                recDat.PaymentJson = tResponse.RawJson;
                                                recDat.Amount = AllConstants.ShareholderAmount;
                                                recDat.Isconfirmed = false;
                                                recDat.TransacRefId = paymentRefCode;
                                                await Db.ShareholdersPayments.AddAsync(recDat);
                                            }
                                            await Db.SaveChangesAsync();
                                            ViewBag.DispMsg = "Payment transaction Initaited,awaiting your bank response,Please try login into your account";
                                            return RedirectToAction("home");
                                        } else ViewBag.DispMsg = "Payment transaction unsuccessfull!";
                                    } else ViewBag.DispMsg = "You have paid for this subcription before.";

                                }


                            }
                        } else ViewBag.DispMsg = "No previous record found for this registration,please start again";
                    }
                } else ViewBag.DispMsg = "No available Session for this registration, please start again";
            } catch {
                ViewBag.DispMsg = "An Internal error has occur please contact the System admin";
            }
            return View();
        }


        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Affiliate(CeoMentor MenReg, SharedHolderNok NokReg, string RegType) {
            Tristate canSave = Tristate.isNo;
            try {
                if (HttpContext.Session.GetInt32("regId") != null) {
                    using (var Db = _context) {
                        var DbRet = await Db.SubscriberRegisters.AsNoTracking()
                            .Where(x => x.Id == HttpContext.Session.GetInt32("regId"))
                            .Select(xRet => new { xRet.Id, xRet.ShareholderIsPaid, xRet.CeomemberIsPaid })
                            .FirstOrDefaultAsync();

                        if (DbRet != null) {
                            if (RegType == RegTypeStruct.Ceomembership) {
                                var RetVal = await Db.CeoMentors.AsNoTracking().AnyAsync(x => x.Id == HttpContext.Session.GetInt32("regId"));
                                MenReg.Id = DbRet.Id;
                                if (RetVal) {
                                    Db.CeoMentors.Update(MenReg);
                                    canSave = Tristate.isBoth;
                                } else {
                                    if (DbRet.CeomemberIsPaid) {
                                        canSave = Tristate.isNo;
                                        ViewBag.DispMsg = "This registration is already completed by you or someone else.";
                                    } else {
                                        await Db.CeoMentors.AddAsync(MenReg);
                                        canSave = Tristate.isYes;
                                    }
                                }
                            } else if (RegType == RegTypeStruct.Shareholder) {
                                var RetVal = await Db.SharedHolderNoks.AsNoTracking().AnyAsync(x => x.Id == HttpContext.Session.GetInt32("regId"));
                                NokReg.Id = DbRet.Id;
                                if (RetVal) {
                                    Db.SharedHolderNoks.Update(NokReg);
                                    canSave = Tristate.isBoth;
                                } else {
                                    if (DbRet.ShareholderIsPaid) {
                                        canSave = Tristate.isNo;
                                        ViewBag.DispMsg = "This registration is already completed by you or someone else.";
                                    } else {
                                        await Db.SharedHolderNoks.AddAsync(NokReg);
                                        canSave = Tristate.isYes;
                                    }
                                }
                            }
                            if (canSave != Tristate.isNo) {
                                await Db.SaveChangesAsync();
                                return RedirectToAction("Registration", new { regtype = RegType, formtype = "account" });
                            }
                        } else ViewBag.DispMsg = "No prevous record found for this registration.";
                    }
                }
            } catch {
                ViewBag.DispMsg = "Unable to process your registration!";
                return View();
            }
            return View();
        }




        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Biodata(SubscriberRegister SubReg, string RegType, IFormFile fotoFile) {
            Tristate canSave = Tristate.isNo;
            try {
                if (string.IsNullOrWhiteSpace(SubReg.ReferralCode)) SubReg.ReferralCode = AllConstants.AppRefCode;
                else if (await new DbAccessClasses(_context).GetRefById(int.Parse(SubReg.ReferralCode.Trim())) == null) {
                    ViewBag.DispMsg = "Invalid Referal Code; Please leave the input blank/empty if not sure.";
                    return View();
                }
                using (var Db = _context) {
                    var DbRet = await Db.SubscriberRegisters.AsNoTracking()
                        .Where(x => x.Email.ToLower() == SubReg.Email)
                        .Select(dtl => new { dtl.Id, dtl.CeomemberIsPaid, dtl.CeomemberIsReg, dtl.ShareholderIsPaid, dtl.ShareholderIsReg })
                        .FirstOrDefaultAsync();

                    //var sVal = DbRet.FirstOrDefault(x => x.CeomemberIsReg);

                    if (DbRet == null) canSave = Tristate.isYes;
                    else {
                        if (RegType == RegTypeStruct.Ceomembership) {
                            if (DbRet.CeomemberIsPaid) canSave = Tristate.isNo;
                            else {
                                SubReg.CeomemberIsReg = true;
                                SubReg.ShareholderIsReg = DbRet.ShareholderIsReg;
                                canSave = Tristate.isBoth;
                            }
                        } else if (RegType == RegTypeStruct.Shareholder) {
                            if (DbRet.ShareholderIsPaid) canSave = Tristate.isNo;
                            else {
                                SubReg.ShareholderIsReg = true;
                                SubReg.CeomemberIsReg = DbRet.CeomemberIsReg;
                                canSave = Tristate.isBoth;
                            }
                        }
                    }

                    // confirm refferalcode later
                    SubReg.RegisteredDate = DateTime.Now;
                    if (canSave == Tristate.isYes) {
                        SubReg.Id = new UtilityClass().GenerateUID();
                        SubReg.PictureName = SubReg.Id.ToString();
                        await Db.SubscriberRegisters.AddAsync(SubReg);
                    } else if (canSave == Tristate.isBoth) {
                        SubReg.Id = DbRet.Id;
                        SubReg.PictureName = SubReg.Id.ToString();
                        Db.SubscriberRegisters.Update(SubReg);
                    }
                    if (canSave != Tristate.isNo) {
                        if (fotoFile != null && fotoFile.Length > 0) {
                            if (await new UtilityClass().SavePix(fotoFile, SubReg.PictureName)) {
                                await Db.SaveChangesAsync();
                                HttpContext.Session.SetInt32("regId", SubReg.Id);
                                return RedirectToAction("Registration", new { regtype = RegType, formtype = "affiliate" });
                            } else ViewBag.DispMsg = "Please upload a valid picture";
                        } else {
                            ViewBag.DispMsg = "Please upload a valid picture";
                        }
                    } else {
                        ViewBag.DispMsg = "This email is already in use by another subcriber";
                    }
                }
            } catch {
                ViewBag.DispMsg = "Unable to process your registration!";
            }
            return View();
        }
        public IActionResult Registration(string regtype, string? formtype) {
            try {
                if (regtype.Trim().Equals("ceomember", StringComparison.OrdinalIgnoreCase) || regtype.Trim().Equals("shareholder", StringComparison.OrdinalIgnoreCase)) {
                    ViewBag.regType = regtype.ToLower();
                    if (!string.IsNullOrEmpty(formtype?.Trim()) && (HttpContext.Session.GetInt32("regId") != null)) {
                        ViewBag.Id = HttpContext.Session.GetInt32("regId");
                        return View(formtype);
                    } else return View("Biodata");
                } else return View("Index");

            } catch {
                return Redirect(Request.Headers["Referer"].ToString());
            }
        }
        public async Task<IActionResult> AutologAuth(string anpd) {
            if (await VerifyLog(anpd.Split('`')[0], anpd.Split()[1])) return RedirectToAction("");
            else return RedirectToAction("Index");
        }

        public IActionResult Index() {
            return View();
        }

        public IActionResult Redirector() {

            //var gg = await new UtilityClass( _context).GetRefById(646445);

            //using (var Db = _context) {
            //var ret = Db.SubscriberRegisters.AsNoTracking().
            //    Where(x => x.Id == 16615628).
            //    Include(ceoMent => ceoMent.CeoMentor)
            //    .Include(shar=>shar.SharedHolderNok)
            //    .Select(sc => new { sc.CeomemberIsPaid, sc.ShareholderIsPaid, sc.CeoMentor,sc.SharedHolderNok })
            //    .FirstOrDefault();

            //var ret = Db.CeoMentors.AsNoTracking().Where(x => x.Id == 16615628).Include(SubscriberRegister => SubscriberRegister.IdNavigation).SingleOrDefault();

            return View();
        }

    }
}


















//}else if (RegType == RegTypeStruct.Ceomembership) {
//    if (DbRet.CeomemberIsReg && DbRet.CeomemberIsPaid) canSave = Tristate.isNo;
//    else if (DbRet.CeomemberIsReg && !DbRet.CeomemberIsPaid) canSave = Tristate.isBoth;
//    else canSave = Tristate.isYes;
//    SubReg.CeomemberIsReg = true;
//} else if (RegType == RegTypeStruct.Shareholder) {
//    if (DbRet.ShareholderIsReg && DbRet.ShareholderIsPaid) canSave = Tristate.isNo;
//    else if (DbRet.ShareholderIsReg && !DbRet.ShareholderIsPaid) canSave = Tristate.isBoth;
//    else canSave = Tristate.isYes;
//    SubReg.ShareholderIsReg = true;
//}



//using (var Db = _context) {
//    var DbRet = await Db.SubscriberRegisters.AsNoTracking()
//        .Where(x => x.Email.ToLower() == SubReg.Email)
//        .Select(dtl => new { dtl.Id, dtl.CeomemberIsPaid, dtl.CeomemberIsReg, dtl.ShareholderIsPaid, dtl.ShareholderIsReg })
//        .ToListAsync();

//    //if(DbRet.Count>2)

//    var sVal = DbRet.FirstOrDefault(x => x.CeomemberIsReg);

//    if (!DbRet.Any()) canSave = Tristate.isYes;
//    else {
//        if (RegType == RegTypeStruct.Ceomembership) {
//            // sVal = DbRet.FirstOrDefault(x => x.CeomemberIsReg);
//            if (sVal != null) {
//                if (sVal.CeomemberIsPaid) canSave = Tristate.isNo;
//                else canSave = Tristate.isBoth;
//            } else canSave = Tristate.isYes;
//        } else if (RegType == RegTypeStruct.Shareholder) {
//            sVal = DbRet.FirstOrDefault(x => x.ShareholderIsReg);
//            if (sVal != null) {
//                if (sVal.ShareholderIsPaid) canSave = Tristate.isNo;
//                else canSave = Tristate.isBoth;
//            } else canSave = Tristate.isYes;
//        }
//    }





//recDat.Id = AcctReg.Id;
//recDat.ReferralCode = DbRet.ReferralCode;
//if (RegType == RegTypeStruct.Ceomembership) {
//    recDat.CeoPaymentDate = DateTime.Now;
//    recDat.CeoPaymentJson = tResponse.RawJson;
//    recDat.Amount = AllConstants.CeomembershipAmount;
//    recDat.CeoIsconfrimed = false;
//    recDat.ShareholderIsconfrimed = payRet.ShareholderIsconfrimed;
//} else {
//    recDat.ShareholderPaymentDate = DateTime.Now;
//    recDat.ShareholderPaymentJson = tResponse.RawJson;
//    recDat.Amount = AllConstants.ShareholderAmount;
//    recDat.ShareholderIsconfrimed = false;
//    recDat.CeoIsconfrimed = payRet.CeoIsconfrimed;
//}
//if (payRet != null) Db.SubcriberPayments.Update(recDat);
//else await Db.SubcriberPayments.AddAsync(recDat);
//await Db.SaveChangesAsync();
//ViewBag.DispMsg = "Payment transaction Initaited,awaiting your bank response,Please try login into your account";
//return RedirectToAction("home");
