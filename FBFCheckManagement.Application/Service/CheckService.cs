using System;
using System.Collections.Generic;
using System.Linq;
using FBFCheckManagement.Application.Domain;
using FBFCheckManagement.Application.DTO;
using FBFCheckManagement.Application.Helper;
using FBFCheckManagement.Application.Repository;

namespace FBFCheckManagement.Application.Service
{
    public class CheckService
    {
        private readonly ICheckRepository _checkRepository;

        public CheckService(ICheckRepository checkRepository){
            _checkRepository = checkRepository;
        }

        public void Add(Check check){
            Check alReadyExistingCheck = _checkRepository.GetCheckByNumber(check.CheckNumber);

            if (alReadyExistingCheck != null 
                & alReadyExistingCheck.Bank.Id == check.Id){
                throw new Exception("Check with this Number already exists");
            }

            _checkRepository.Add(check);
        }

        public void Update(Check check){
            Check oldCheck = _checkRepository.GetCheckByNumber(check.CheckNumber);

            oldCheck.CheckNumber = check.CheckNumber;
            oldCheck.Bank = check.Bank;
            oldCheck.DateIssued = check.DateIssued;
            oldCheck.Amount = check.Amount;
            oldCheck.IssuedTo = check.IssuedTo;
            oldCheck.ModifiedDate = DateTime.Now;

            _checkRepository.Update(oldCheck);
        }

        public decimal ComputeChecksTotalInDay(DateTime selectedDay, List<Check> checks){
            decimal total = 0;
            var checksWithinThatDay =
                checks.Where(
                    c =>
                        c.DateIssued.HasValue && c.DateIssued.Value.Year == selectedDay.Year &&
                        c.DateIssued.Value.Month == selectedDay.Month && c.DateIssued.Value.Day == selectedDay.Day).ToList();

            foreach (var c in checksWithinThatDay){
                total = total + c.Amount;
            }

            return total;
        }

        public decimal ComputeChecksTotalInTheWeek(DateTime dayWithinAWeek, List<Check> checks){
            decimal total = 0;

            DateTime firstDayOfTheWeek = dayWithinAWeek.GetFirstDayOfWeek();
            DateTime lastDayOfTheWeek = dayWithinAWeek.GetLastDayOfWeek();

            var checksWithinThisWeek = checks.Where(
                c =>
                    (c.DateIssued.HasValue && c.DateIssued.Value >= firstDayOfTheWeek &&
                    c.DateIssued.Value <= lastDayOfTheWeek)
                    &&
                    !c.HoldDate.HasValue).ToList();

            var onHoldChecksWithinThisWeek = checks.Where(
                c =>
                    c.HoldDate.HasValue && c.HoldDate.Value >= firstDayOfTheWeek &&
                    c.HoldDate.Value <= lastDayOfTheWeek
                ).ToList();

            checksWithinThisWeek.AddRange(onHoldChecksWithinThisWeek);

            foreach (var c in checksWithinThisWeek){
                total = total + c.Amount;
            }

            return total;
        }

        public CheckPagingResult SearchWithPagination(CheckPagingRequest r){
            CheckPagingResult result = new CheckPagingResult();
            result = _checkRepository.GetCheckWithPaging(r);
            return result;
        }
    }
}