using Talent.Common.Contracts;
using Talent.Common.Models;
using Talent.Services.Profile.Domain.Contracts;
using Talent.Services.Profile.Models.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using Talent.Services.Profile.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Talent.Common.Security;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Internal.System.Collections.Sequences;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Talent.Services.Profile.Domain.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserAppContext _userAppContext;
        IRepository<UserLanguage> _userLanguageRepository;
        IRepository<User> _userRepository;
        IRepository<Employer> _employerRepository;
        IRepository<Job> _jobRepository;
        IRepository<Recruiter> _recruiterRepository;
        IFileService _fileService;


        public ProfileService(IUserAppContext userAppContext,
                              IRepository<UserLanguage> userLanguageRepository,
                              IRepository<User> userRepository,
                              IRepository<Employer> employerRepository,
                              IRepository<Job> jobRepository,
                              IRepository<Recruiter> recruiterRepository,
                              IFileService fileService)
        {
            _userAppContext = userAppContext;
            _userLanguageRepository = userLanguageRepository;
            _userRepository = userRepository;
            _employerRepository = employerRepository;
            _jobRepository = jobRepository;
            _recruiterRepository = recruiterRepository;
            _fileService = fileService;
        }

        public async Task<AddSkillViewModel> AddNewSkillAsync(AddSkillViewModel skill, string currentUserId)
        {
            try
            {
                if (!string.IsNullOrEmpty(currentUserId))
                {
                    var user = await _userRepository.GetByIdAsync(currentUserId);

                    if (user != null && skill != null)
                    {
                        skill.Id = ObjectId.GenerateNewId().ToString();

                        var userkill = new UserSkill();
                        userkill.Id = skill.Id;
                        userkill.Skill = skill.Name;
                        userkill.ExperienceLevel = skill.Level;
                        userkill.IsDeleted = false;
                        userkill.UserId = currentUserId;

                        user.Skills.Add(userkill);
                        await _userRepository.Update(user);
                        return skill;
                    }
                    return null;  
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while adding new skill: " + ex.Message);
                return null;
            }
        }

        public async Task<bool> DeleteSkillAsync(string id, string currentUserId)
        {
            try
            {
                if (!string.IsNullOrEmpty(currentUserId) && !string.IsNullOrEmpty(id))
                {
                    var user = await _userRepository.GetByIdAsync(currentUserId);
                    var skill = user.Skills.Find(skll => skll.Id == id);
                    if (user != null && skill != null)
                    {
                        user.Skills.Remove(skill);
                        await _userRepository.Update(user);
                        return true;
                    }
                    return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while deleting skill: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> EditSkillAsync(AddSkillViewModel skill, string id, string currentUserId)
        {
            try
            {
                if (!string.IsNullOrEmpty(currentUserId) && !string.IsNullOrEmpty(id))
                {
                    var user = await _userRepository.GetByIdAsync(currentUserId);
                    var skills = user.Skills.SingleOrDefault(x => x.Id == id);
                    if (user != null && skill != null)
                    {
                        UpdateSkillFromView(skill, skills);
                        await _userRepository.Update(user);
                        return true;
                    }
                    return false;
                }
                return false;
            }
            catch(Exception ex)
            {
                Console.WriteLine("An error occurred while editing skill: " + ex.Message);
                return false;
            }
        }

        public async Task<ExperienceViewModel> AddNewExperienceAsync(ExperienceViewModel experience, string currentUserId)
        {
            try
            {
                if (!string.IsNullOrEmpty(currentUserId))
                {
                    var user = await _userRepository.GetByIdAsync(currentUserId);
                    if (user != null && experience != null)
                    {
                        experience.Id = ObjectId.GenerateNewId().ToString();
                        var usexp = new UserExperience();
                        usexp.Id = experience.Id;
                        usexp.Company = experience.Company;
                        usexp.Position = experience.Position;
                        usexp.Start = experience.Start;
                        usexp.End = experience.End;
                        usexp.Responsibilities = experience.Responsibilities;

                        user.Experience.Add(usexp);
                        await _userRepository.Update(user);
                        return experience;
                    }
                    return null;
                }
                return null;
            }
            catch(Exception ex)
            {
                Console.WriteLine("An error occurred while adding new experience: " + ex.Message);
                return null;
            }   
        }

        public async Task<bool> DeleteExperienceAsync(string id, string currentUserId)
        {
            try
            {
                if (!string.IsNullOrEmpty(currentUserId) && !string.IsNullOrEmpty(id))
                {
                    var user = await _userRepository.GetByIdAsync(currentUserId);
                    var experience = user.Experience.Find(exp => exp.Id == id);
                    if (user != null && experience != null)
                    {
                        user.Experience.Remove(experience);
                        await _userRepository.Update(user);
                        return true;
                    }
                    return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while deleting experience: " + ex.Message);
                return false;
            }

        }

        public async Task<bool> EditExperienceAsync(ExperienceViewModel experience, string id, string currentUserId)
        {
            try
            {
                if (!string.IsNullOrEmpty(currentUserId) && !string.IsNullOrEmpty(id))
                {
                    var user = await _userRepository.GetByIdAsync(currentUserId);
                    var experiences = user.Experience.SingleOrDefault(x => x.Id == id);
                    if (user != null && experience != null)
                    {
                        UpdateExperienceFromView(experience, experiences);
                        await _userRepository.Update(user);
                        return true;
                    }
                    return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while editing experience: " + ex.Message);
                return false;
            }
        }

        public async Task<AddLanguageViewModel> AddNewLanguageAsync(AddLanguageViewModel language, string currentUserId)
        {
            try
            {
                if (!string.IsNullOrEmpty(currentUserId))
                {
                    var user = await _userRepository.GetByIdAsync(currentUserId);
                    if (user != null && language != null)
                    {
                        language.Id = ObjectId.GenerateNewId().ToString();
                        language.CurrentUserId = currentUserId;
                        var ulang = new UserLanguage();
                        ulang.Id = language.Id;
                        ulang.Language = language.Name;
                        ulang.LanguageLevel = language.Level;
                        ulang.IsDeleted = false;
                        ulang.UserId = language.CurrentUserId;
                        user.Languages.Add(ulang);
                        await _userRepository.Update(user);
                        return language;
                    }
                    return null;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while adding new language: " + ex.Message);
                return null;
            }
        }

        public async Task<bool> DeleteLanguageAsync(string id, string currentUserId)
        {
            try
            {
                if (!string.IsNullOrEmpty(currentUserId) && !string.IsNullOrEmpty(id))
                {
                    var user = await _userRepository.GetByIdAsync(currentUserId);
                    var language = user.Languages.Find(lang => lang.Id == id);
                    if (user != null && language != null)
                    {
                        user.Languages.Remove(language);
                        await _userRepository.Update(user);
                        return true;
                    }
                    return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while deleting language: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> EditLanguageAsync(AddLanguageViewModel language, string id, string currentUserId)
        {
            try
            {
                if (!string.IsNullOrEmpty(currentUserId) && !string.IsNullOrEmpty(id))
                {
                    var user = await _userRepository.GetByIdAsync(currentUserId);
                    var languages = user.Languages.SingleOrDefault(x => x.Id == id);
                    if (user != null && language != null)
                    {
                        UpdatelanguageFromView(language, languages);
                        await _userRepository.Update(user);
                        return true;
                    }
                    return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while editing language: " + ex.Message);
                return false;
            }
        }

        public async Task<TalentProfileViewModel> GetTalentProfileAsync(string Id)
        {
            try
            {
                if (!string.IsNullOrEmpty(Id))
                {
                    User Talentprofile = null;
                    Talentprofile = await _userRepository.GetByIdAsync(Id);
                    var languages = Talentprofile.Languages.Select(x => ViewModelFromLanguage(x)).ToList();
                    var skills = Talentprofile.Skills.Select(y => ViewModelFromSkill(y)).ToList();
                    var experience = Talentprofile.Experience.Select(z => ViewModelFromExperience(z)).ToList();
                    if (Talentprofile != null)
                    {
                        var result = new TalentProfileViewModel
                        {
                            Id = Talentprofile.Id,
                            FirstName = Talentprofile.FirstName,
                            LastName = Talentprofile.LastName,
                            Email = Talentprofile.Email,
                            Phone = Talentprofile.Phone,
                            LinkedAccounts = Talentprofile.LinkedAccounts,
                            Address = Talentprofile.Address,
                            Nationality = Talentprofile.Nationality,
                            Summary = Talentprofile.Summary,
                            Description = Talentprofile.Description,
                            JobSeekingStatus = Talentprofile.JobSeekingStatus,
                            VisaStatus = Talentprofile.VisaStatus,
                            VisaExpiryDate = Talentprofile.VisaExpiryDate,
                            ProfilePhoto = Talentprofile.ProfilePhoto,
                            ProfilePhotoUrl = Talentprofile.ProfilePhotoUrl,
                            Languages = languages,
                            Skills = skills,
                            Experience = experience
                        };
                        return result;
                    }
                    return null;
                }
                return null;
            }
            catch(Exception ex)
            {
                Console.WriteLine("An error occurred while getting talent profile: " + ex.Message);
                return null;
            }
        }

        public async Task<bool> UpdateTalentProfileAsync(TalentProfileViewModel model, string updaterId)
        {
            try
            {
                if (!string.IsNullOrEmpty(updaterId))
                {
                    var user = await _userRepository.GetByIdAsync(updaterId);
                    if (user != null)
                    {
                        user.LinkedAccounts = model.LinkedAccounts;
                        user.FirstName = model.FirstName;
                        user.LastName = model.LastName;
                        user.Email = model.Email;
                        user.Phone = model.Phone;
                        user.Address = model.Address;
                        user.Nationality = model.Nationality;
                        user.Summary = model.Summary;
                        user.Description = model.Description;
                        user.JobSeekingStatus = model.JobSeekingStatus;
                        user.VisaStatus = model.VisaStatus;
                        user.VisaExpiryDate = model.VisaExpiryDate;
                        user.ProfilePhoto = model.ProfilePhoto;
                        user.ProfilePhotoUrl = model.ProfilePhotoUrl;
                        await _userRepository.Update(user);
                        return true;
                    }
                    return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while updating talent profile: " + ex.Message);
                return false;
            }
        }

        public async Task<EmployerProfileViewModel> GetEmployerProfile(string Id, string role)
        {

            Employer profile = null;
            switch (role)
            {
                case "employer":
                    profile = (await _employerRepository.GetByIdAsync(Id));
                    break;
                case "recruiter":
                    profile = (await _recruiterRepository.GetByIdAsync(Id));
                    break;
            }

            var videoUrl = "";

            if (profile != null)
            {
                videoUrl = string.IsNullOrWhiteSpace(profile.VideoName)
                          ? ""
                          : await _fileService.GetFileURL(profile.VideoName, FileType.UserVideo);

                var skills = profile.Skills.Select(x => ViewModelFromSkill(x)).ToList();

                var result = new EmployerProfileViewModel
                {
                    Id = profile.Id,
                    CompanyContact = profile.CompanyContact,
                    PrimaryContact = profile.PrimaryContact,
                    Skills = skills,
                    ProfilePhoto = profile.ProfilePhoto,
                    ProfilePhotoUrl = profile.ProfilePhotoUrl,
                    VideoName = profile.VideoName,
                    VideoUrl = videoUrl,
                    DisplayProfile = profile.DisplayProfile,
                };
                return result;
            }

            return null;
        }

        public async Task<bool> UpdateEmployerProfile(EmployerProfileViewModel employer, string updaterId, string role)
        {
            try
            {
                if (employer.Id != null)
                {
                    switch (role)
                    {
                        case "employer":
                            Employer existingEmployer = (await _employerRepository.GetByIdAsync(employer.Id));
                            existingEmployer.CompanyContact = employer.CompanyContact;
                            existingEmployer.PrimaryContact = employer.PrimaryContact;
                            existingEmployer.ProfilePhoto = employer.ProfilePhoto;
                            existingEmployer.ProfilePhotoUrl = employer.ProfilePhotoUrl;
                            existingEmployer.DisplayProfile = employer.DisplayProfile;
                            existingEmployer.UpdatedBy = updaterId;
                            existingEmployer.UpdatedOn = DateTime.Now;

                            var newSkills = new List<UserSkill>();
                            foreach (var item in employer.Skills)
                            {
                                var skill = existingEmployer.Skills.SingleOrDefault(x => x.Id == item.Id);
                                if (skill == null)
                                {
                                    skill = new UserSkill
                                    {
                                        Id = ObjectId.GenerateNewId().ToString(),
                                        IsDeleted = false
                                    };
                                }
                                UpdateSkillFromView(item, skill);
                                newSkills.Add(skill);
                            }
                            existingEmployer.Skills = newSkills;

                            await _employerRepository.Update(existingEmployer);
                            break;

                        case "recruiter":
                            Recruiter existingRecruiter = (await _recruiterRepository.GetByIdAsync(employer.Id));
                            existingRecruiter.CompanyContact = employer.CompanyContact;
                            existingRecruiter.PrimaryContact = employer.PrimaryContact;
                            existingRecruiter.ProfilePhoto = employer.ProfilePhoto;
                            existingRecruiter.ProfilePhotoUrl = employer.ProfilePhotoUrl;
                            existingRecruiter.DisplayProfile = employer.DisplayProfile;
                            existingRecruiter.UpdatedBy = updaterId;
                            existingRecruiter.UpdatedOn = DateTime.Now;

                            var newRSkills = new List<UserSkill>();
                            foreach (var item in employer.Skills)
                            {
                                var skill = existingRecruiter.Skills.SingleOrDefault(x => x.Id == item.Id);
                                if (skill == null)
                                {
                                    skill = new UserSkill
                                    {
                                        Id = ObjectId.GenerateNewId().ToString(),
                                        IsDeleted = false
                                    };
                                }
                                UpdateSkillFromView(item, skill);
                                newRSkills.Add(skill);
                            }
                            existingRecruiter.Skills = newRSkills;
                            await _recruiterRepository.Update(existingRecruiter);

                            break;
                    }
                    return true;
                }
                return false;
            }
            catch (MongoException e)
            {
                return false;
            }
        }

        public async Task<bool> UpdateEmployerPhoto(string employerId, IFormFile file)
        {
            var fileExtension = Path.GetExtension(file.FileName);
            List<string> acceptedExtensions = new List<string> { ".jpg", ".png", ".gif", ".jpeg" };

            if (fileExtension != null && !acceptedExtensions.Contains(fileExtension.ToLower()))
            {
                return false;
            }

            var profile = (await _employerRepository.Get(x => x.Id == employerId)).SingleOrDefault();

            if (profile == null)
            {
                return false;
            }

            var newFileName = await _fileService.SaveFile(file, FileType.ProfilePhoto);

            if (!string.IsNullOrWhiteSpace(newFileName))
            {
                var oldFileName = profile.ProfilePhoto;

                if (!string.IsNullOrWhiteSpace(oldFileName))
                {
                    await _fileService.DeleteFile(oldFileName, FileType.ProfilePhoto);
                }

                profile.ProfilePhoto = newFileName;
                profile.ProfilePhotoUrl = await _fileService.GetFileURL(newFileName, FileType.ProfilePhoto);

                await _employerRepository.Update(profile);
                return true;
            }

            return false;

        }

        public async Task<bool> AddEmployerVideo(string employerId, IFormFile file)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateTalentPhotoAsync(string talentId, IFormFile file)
        {
            try
            {
                if (!string.IsNullOrEmpty(talentId))
                {
                    var fileExtension = Path.GetExtension(file.FileName);
                    List<string> acceptedExtensions = new List<string> { ".jpg", ".png", ".gif", ".jpeg" };
                    if (fileExtension != null && !acceptedExtensions.Contains(fileExtension.ToLower()))
                    {
                        return false;
                    }

                    var talentProfile = (await _userRepository.Get(x => x.Id == talentId)).SingleOrDefault();
                    if (talentProfile == null)
                    {
                        return false;
                    }
                    var newFileName = await _fileService.SaveFile(file, FileType.ProfilePhoto);

                    if (!string.IsNullOrWhiteSpace(newFileName))
                    {
                        var oldFileName = talentProfile.ProfilePhoto;

                        if (!string.IsNullOrWhiteSpace(oldFileName))
                        {
                            await _fileService.DeleteFile(oldFileName, FileType.ProfilePhoto);
                        }

                        talentProfile.ProfilePhoto = newFileName;
                        talentProfile.ProfilePhotoUrl = await _fileService.GetFileURL(newFileName, FileType.ProfilePhoto);

                        await _userRepository.Update(talentProfile);
                        return true;
                    }
                    return false;
                }
                return false;
            }
            catch(Exception ex)
            {
                Console.WriteLine("An error occurred while updating talent photo: " + ex.Message);
                return false;

            }
        }

        public async Task<bool> AddTalentVideo(string talentId, IFormFile file)
        {
            //Your code here;
            throw new NotImplementedException();

        }

        public async Task<bool> RemoveTalentVideo(string talentId, string videoName)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateTalentCV(string talentId, IFormFile file)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<string>> GetTalentSuggestionIds(string employerOrJobId, bool forJob, int position, int increment)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TalentSnapshotViewModel>> GetTalentSnapshotListAsync(string employerOrJobId, bool forJob, int position, int increment)
        {
            try
            {
                
                IQueryable<User> query = _userRepository.GetQueryable();
                query = query.Where(u => !u.IsDeleted);
                query = query.OrderByDescending(u => u.CreatedOn);
                query = query.Take(increment);

                IEnumerable<User> users = query.ToList();

                var talentSnapshots = users.Select(user =>
                {
                    
                    var currentEmployment = ""; 
                    var level = ""; 
                    var skills = user.Skills?.Select(skill => skill.Skill).ToList() ?? new List<string>();

                    return new TalentSnapshotViewModel
                    {
                        Id = user.Id,
                        Name = $"{user.FirstName} {user.LastName}",
                        PhotoId = user.ProfilePhotoUrl,
                        VideoUrl = user.VideoName,
                        CVUrl = user.CvName,
                        Summary = user.Summary,
                        Visa = user.VisaStatus,
                        CurrentEmployment = currentEmployment,
                        Level = level,
                        Skills = skills
                    };
                }).ToList();

                return talentSnapshots;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<IEnumerable<TalentSnapshotViewModel>> GetTalentSnapshotList(IEnumerable<string> ids)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        #region TalentMatching

        public async Task<IEnumerable<TalentSuggestionViewModel>> GetFullTalentList()
        {
            //Your code here;
            throw new NotImplementedException();
        }

        public IEnumerable<TalentMatchingEmployerViewModel> GetEmployerList()
        {
            //Your code here;
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TalentMatchingEmployerViewModel>> GetEmployerListByFilterAsync(SearchCompanyModel model)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TalentSuggestionViewModel>> GetTalentListByFilterAsync(SearchTalentModel model)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TalentSuggestion>> GetSuggestionList(string employerOrJobId, bool forJob, string recruiterId)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        public async Task<bool> AddTalentSuggestions(AddTalentSuggestionList selectedTalents)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        #endregion

        #region Conversion Methods

        #region Update from View

        protected void UpdateSkillFromView(AddSkillViewModel model, UserSkill original)
        {
            original.ExperienceLevel = model.Level;
            original.Skill = model.Name;
        }

        #endregion

        #region Build Views from Model

        protected AddSkillViewModel ViewModelFromSkill(UserSkill skill)
        {
            return new AddSkillViewModel
            {
                Id = skill.Id,
                Level = skill.ExperienceLevel,
                Name = skill.Skill,
                
            };
        }

        protected void UpdatelanguageFromView(AddLanguageViewModel languagemodel, UserLanguage original)
        {
            original.Language = languagemodel.Name;
            original.LanguageLevel = languagemodel.Level;
        }

        protected AddLanguageViewModel ViewModelFromLanguage(UserLanguage lang)
        {
            return new AddLanguageViewModel
            {
                Id = lang.Id,
                Level = lang.LanguageLevel,
                Name = lang.Language,
                CurrentUserId = lang.UserId
            };
        }

        protected void UpdateExperienceFromView(ExperienceViewModel expmodel, UserExperience original)
        {
            original.Company = expmodel.Company;
            original.Position = expmodel.Position;
            original.Start = expmodel.Start;
            original.End = expmodel.End;
            original.Responsibilities = expmodel.Responsibilities;
        }

        protected ExperienceViewModel ViewModelFromExperience(UserExperience exp)
        {
            return new ExperienceViewModel
            {
                Id = exp.Id,
                Company = exp.Company,
                Position = exp.Position,
                Start = exp.Start,
                End = exp.End,
                Responsibilities = exp.Responsibilities,
            };
        }

        #endregion

        #endregion

        #region ManageClients

        public async Task<IEnumerable<ClientViewModel>> GetClientListAsync(string recruiterId)
        {
            //Your code here;
            throw new NotImplementedException();
        }

        public async Task<ClientViewModel> ConvertToClientsViewAsync(Client client, string recruiterId)
        {
            //Your code here;
            throw new NotImplementedException();
        }
         
        public async Task<int> GetTotalTalentsForClient(string clientId, string recruiterId)
        {
            //Your code here;
            throw new NotImplementedException();

        }

        public async Task<Employer> GetEmployer(string employerId)
        {
            return await _employerRepository.GetByIdAsync(employerId);
        }
        #endregion

    }
}
