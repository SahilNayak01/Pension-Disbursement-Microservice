using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


//change get to post

namespace PensionDisbursement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PensionDisbursementController : ControllerBase
    {
        public IConfiguration configuration;
        public PensionDisbursementController(IConfiguration iConfig)
        {
            configuration = iConfig;
        }
        [HttpPost]
        public int GetDisbursePension(ProcessPensionInput pension)
        {
            PensionerDetail pensionerDetail = new PensionerDetail();
            ServiceWrapper getPensionerDetail = new ServiceWrapper(configuration);

            pensionerDetail = getPensionerDetail.GetDetailResponse(pension.AadharNumber);


            if (pensionerDetail == null)
                return 21;

            int status = 0;
            int bankServiceCharge;
            if (pension.BankType == 1)
                bankServiceCharge = 500;
            else if (pension.BankType == 2)
                bankServiceCharge = 550;
            else
                bankServiceCharge = 0;
            double pensionCalculated;
            pensionCalculated = CalculatePensionLogic(pensionerDetail.SalaryEarned, pensionerDetail.Allowances, bankServiceCharge, pensionerDetail.PensionType);

            if (Convert.ToDouble(pension.PensionAmount) == pensionCalculated)
            {
                status = 10;
            }
            else
            {
                status = 21;
            }
            return status;
            
        }
        private double CalculatePensionLogic(int salaryEarned,int allowances,int charge,PensionType type)
        {
            if (type == PensionType.Self)
            {
                return (salaryEarned * 0.8) + allowances + charge;
            }
            else
            {
                return (salaryEarned * 0.5) + allowances + charge;
            }

        }
        

    }
}
