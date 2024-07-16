using Talent.Services.Profile.Models.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Talent.Services.Profile.Models;
using Talent.Common.Models;
using Microsoft.AspNetCore.Http;

namespace Talent.Services.Profile.Domain.Contracts
{
    public interface IProfileService
    {
        Task<AddSkillViewModel> AddNewSkillAsync(AddSkillViewModel skill, string currentUserId);

        Task<bool> DeleteSkillAsync(string id, string currentUserId);

        Task<bool> EditSkillAsync(AddSkillViewModel skill, string id, string currentUserId);

        Task<AddLanguageViewModel> AddNewLanguageAsync(AddLanguageViewModel language, string currentUserId);

        Task<bool> DeleteLanguageAsync(string id, string currentUserId);

        Task<bool> EditLanguageAsync(AddLanguageViewModel language, string id, string currentUserId);

        Task<ExperienceViewModel> AddNewExperienceAsync(ExperienceViewModel experience, string currentUserId);

        Task<bool> DeleteExperienceAsync(string id, string currentUserId);

        Task<bool> EditExperienceAsync(ExperienceViewModel experience, string id, string currentUserId);

        Task<TalentProfileViewModel> GetTalentProfileAsync(string Id);
        Task<IEnumerable<string>> GetTalentSuggestionIds(string employerOrJobId, bool forJob, int position, int increment);
        Task<IEnumerable<TalentSnapshotViewModel>> GetTalentSnapshotListAsync(string employerOrJobId, bool forJob, int position, int increment);
        Task<IEnumerable<TalentSnapshotViewModel>> GetTalentSnapshotList(IEnumerable<string> ids);

        Task<bool> UpdateTalentProfileAsync(TalentProfileViewModel profile, string updaterId);
        Task<bool> UpdateTalentPhotoAsync(string talentId, IFormFile file);

        Task<bool> AddTalentVideo(string talentId, IFormFile file);
        Task<bool> RemoveTalentVideo(string talentId, string videoName);

        Task<EmployerProfileViewModel> GetEmployerProfile(string id, string role);

        Task<bool> UpdateEmployerProfile(EmployerProfileViewModel profile, string updaterId, string role);
        Task<bool> UpdateEmployerPhoto(string employerId, IFormFile file);

        Task<bool> AddEmployerVideo(string employerId, IFormFile file);
        Task<bool> AddTalentSuggestions(AddTalentSuggestionList selectedTalents);

        Task<bool> UpdateTalentCV(string talentId, IFormFile file);

        Task<IEnumerable<TalentSuggestionViewModel>> GetFullTalentList();
        IEnumerable<TalentMatchingEmployerViewModel> GetEmployerList();
        Task<IEnumerable<TalentMatchingEmployerViewModel>> GetEmployerListByFilterAsync(SearchCompanyModel model);
        Task<IEnumerable<TalentSuggestion>> GetSuggestionList(string employerOrJobId, bool forJob, string recruiterId);
        Task<IEnumerable<TalentSuggestionViewModel>> GetTalentListByFilterAsync(SearchTalentModel model);

        Task<IEnumerable<ClientViewModel>> GetClientListAsync(string recruiterId);
        Task<Employer> GetEmployer(string employerId);
    }
}
