﻿using System;
using System.Collections.Generic;
using System.Linq;
using FBFCheckManagement.Application.Domain;
using FBFCheckManagement.Application.Repository;
using FBFCheckManagement.Infrastructure.EntityFramework;

namespace FBFCheckManagement.Infrastructure.Repository
{
    public class BankRepository : IBankRepository
    {
        private readonly FBFDbContext _context;
        private string _statusMessage = string.Empty;
        private bool _isSuccess;

        public BankRepository(IDatabaseType databaseType){
            if (databaseType == null)
                throw new ArgumentNullException("databaseType is null");
            _context = new FBFDbContext(databaseType);
        }

        public void AddBank(long departmentId, Bank bank){
            var parentDepartment = _context.Departments.FirstOrDefault(d => d.Id == departmentId);
            if (parentDepartment != null){
                bool isExist = parentDepartment.Banks.Any(b => b.BankName == bank.BankName);

                if (isExist != true){

                    parentDepartment.Banks.Add(bank);
                    _context.SaveChanges();
                    _isSuccess = true;
                    _statusMessage = "success adding bank";
                }
                else{
                    _isSuccess = false;
                    _statusMessage = "failed adding bank: bank already exist";
                }
            }
            else{
                _isSuccess = false;
                _statusMessage = "failed adding bank: parent department does not exist";
            }
        }

        public List<Bank> GetAllBanks(){
            return _context.Banks.ToList();
        }

        public List<Bank> GetBanksByDepartment(long departmentId){
            return _context.Banks.Where(b => b.Department.Id == departmentId).ToList();
        }

        public void EditBank(Bank bankToEdit){
            bool isExist = _context.Banks.Any(b => b.BankName == bankToEdit.BankName);

            if (isExist == false){
                Bank oldBank = _context.Banks.Include("Department").FirstOrDefault(b => b.Id == bankToEdit.Id);

                oldBank.BankName = bankToEdit.BankName;
                oldBank.ModifiedDate = DateTime.Now;
             
                _context.SaveChanges();

                _isSuccess = true;
                _statusMessage = "success editing Bank";
            }
            else{
                _isSuccess = false;
                _statusMessage = "failed editing bank: bank already exist";
            }
        }

        public Bank GetBankById(int id){
            return _context.Banks.FirstOrDefault(b => b.Id == id);
        }

        public Bank GetBankByName(string name){
            return _context.Banks.FirstOrDefault(b => b.BankName == name);
        }

        public string StatusMessage{
            get { return _statusMessage; }
        }

        public bool IsSuccess{
            get { return _isSuccess; }
        }
    }
}