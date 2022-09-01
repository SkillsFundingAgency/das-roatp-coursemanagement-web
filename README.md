# ![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png) Digital Apprenticeships Service

##  RoATP Course Management UI

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">


## About

The front end for main providers to view and administer the courses they offer, and the venues or subregions where the courses are offered, and the provider description.

It interacts with an outer api (https://github.com/SkillsFundingAgency/das-apim-endpoints/tree/master/src/RoatpCourseManagement), which in turn interacts with several other sources of data, but most significantly the inner api for roatp data (https://github.com/SkillsFundingAgency/das-roatp-api)


### Developer Setup

#### Requirements
- Clone this repository
- Install [Visual Studio 2022](https://www.visualstudio.com/downloads/) with these workloads:
    - ASP.NET and web development
    - Azure development
- Install [Azure Storage Emulator]
- Install [Azure Storage Explorer](http://storageexplorer.com/)
- Administrator Access

#### Setup

- Create a Configuration table in your (Development) local storage account.
- Obtain the local config json from the das-employer-config for das-roatp-coursemanagement-web repo (https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-roatp-course-management-web/SFA.DAS.Roatp.CourseManagement.Web.json) 
  - PartitionKey: LOCAL
  - RowKey: SFA.DAS.Roatp.CourseManagement.Web_1.0
  - Data: {The contents of the local config json file}
  
You will also need to setup the roatp outer api and have it running (see https://github.com/SkillsFundingAgency/das-apim-endpoints/ and go to the section for 'Course Management')

Open the solution with Visual Studio, and run the project SFA.DAS.Roatp.CourseManagement.Web, running under process 'SFA.DAS.Roatp.CourseManagement.Web' (not IIS)


  