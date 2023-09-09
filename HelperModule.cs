using AppnetworkCEO2023.Models;
using Microsoft.EntityFrameworkCore;
using PayStack.Net;
using System.Text;


namespace HelperModule.net {
    public class PayStackExtendedClass {
        private static readonly PayStackApi _api = new("sk_test_4d565d7c4e00fc223282a2d828b793d4736e6768");
        public TransactionInitializeResponse InitializePaymentRequest(int nairaAmnt, string eMail, string refCod, string Payer, string payDetail, string veriCallBack) {
            var request = new TransactionInitializeRequest {
                AmountInKobo = nairaAmnt * 100,  // 900000,
                Email = eMail,
                Reference = refCod, // or your custom reference
                CallbackUrl = "http://appnetworkltd.com/home/" + "PaymentVerificationCallBack"
            };
            request.CustomFields.Add(CustomField.From("Name", "name", Payer));  // Add customer fields          
            request.MetadataObject["DataKey"] = payDetail;  // Add other metadata
            var response = _api.Transactions.Initialize(request);
            return response;
        }

    }

    public class DbAccessClasses {

        private readonly AppnetworkCeodbContext _context;
        public DbAccessClasses(AppnetworkCeodbContext context) {
            _context = context;
        }

        public async Task<SubscriberRegister?> GetRefById(int id) {
            try {
                var rVal = await _context.SubscriberRegisters.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
                return rVal;
            } catch { return null; }
        }
    }




    public class UtilityClass {
        public enum Tristate { isYes = 1, isNo = 2, isBoth = 3 }
        public struct AllConstants {
            public const string AppRefCode = "12345678";
            public const int CeomembershipAmount = 60000;
            public const int ShareholderAmount = 60000;
        }
        public struct RegTypeStruct {
            public const string Ceomembership = "ceomember";
            public const string Shareholder = "shareholder";
            public const string Both = "both";
        }

        public string CodeString(string dStr, bool encodeIt) {
            if (encodeIt) return Convert.ToBase64String(Encoding.ASCII.GetBytes(dStr));
            else return Encoding.UTF8.GetString(Convert.FromBase64String(dStr));
        }




        public int GenerateUID(ushort Ulenght = 8, ushort maxOption = 10) {
            var sb = new System.Text.StringBuilder();
            var rnd = new Random();
            string[] AlphaNumb = "0,1,2,3,4,5,6,7,8,9".Split(',');
            for (ushort i = 0; i < Ulenght; i++) sb.Append(AlphaNumb[rnd.Next(maxOption)]);
            return (int.Parse(sb.ToString()));
        }
        public async Task<bool> SavePix(IFormFile? PixFile, string PixName) {
            try {
                var path = Path.Combine(Environment.CurrentDirectory, PixName);
                using (FileStream stream = new FileStream(path, FileMode.Create)) {
                    await PixFile.CopyToAsync(stream);
                    stream.Close();
                }
                return true;
            } catch {
                return false;
            }
        }
    }
}
//,A,B,C,D,E,F,G,H,I,J,KL,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z