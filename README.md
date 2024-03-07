# ![crest](https://assets.publishing.service.gov.uk/government/assets/crests/org_crest_27px-916806dcf065e7273830577de490d5c7c42f36ddec83e907efe62086785f24fb.png) Digital Apprenticeships Service

##  RoATP Course Management UI

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_apis/build/status/SkillsFundingAgency_das-roatp-coursemanagement-web?branchName=master)](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_build/latest?definitionId=SkillsFundingAgency_das-roatp-coursemanagement-web&branchName=master)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SkillsFundingAgency_das-roatp-coursemanagement-web&metric=alert_status)](https://sonarcloud.io/dashboard?id=SkillsFundingAgency_das-roatp-coursemanagement-web)
[![Confluence Page](https://img.shields.io/badge/Confluence-Project-blue)](https://skillsfundingagency.atlassian.net/wiki/spaces/NDL/pages/3867705345/AAN+Employer+Solution+Architecture)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)


## About

The front end for providers to view and administer the courses they offer, and the venues or subregions where the courses are offered, and the provider description.

It interacts with an outer api (https://github.com/SkillsFundingAgency/das-apim-endpoints/tree/master/src/RoatpCourseManagement), which in turn interacts with several other sources of data, but most significantly the inner api for roatp data (https://github.com/SkillsFundingAgency/das-roatp-api)


### Developer Setup

#### Requirements
- Clone this repository
- Install [Visual Studio 2022](https://www.visualstudio.com/downloads/) with these workloads:
    - ASP.NET and web development
    - Azure development
- Install [Azure Storage Emulator]
- Install [Azure Storage Explorer](http://storageexplorer.com/)

#### Setup

- Create a Configuration table in your (Development) local storage account.
- Obtain the local config json from the das-employer-config for das-roatp-coursemanagement-web repo (https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-roatp-course-management-web/SFA.DAS.Roatp.CourseManagement.Web.json) 
  - PartitionKey: LOCAL
  - RowKey: SFA.DAS.Roatp.CourseManagement.Web_1.0
  - Data: {The contents of the local config json file}
  
In the web project, if not exist already, add `AppSettings.Development.json` file with following content:
```json
{
  "RoatpCourseManagement": {
    "RedisConnectionString": " ",
    "DataProtectionKeysDatabase": "",
    "UseDfESignIn": false
  },
  "RoatpCourseManagementOuterApi": {
    "BaseUrl": "http://localhost:5335/",
    "SubscriptionKey": "Key",
    "PingUrl": "http://localhost:5335/"
  },
  "ProviderSharedUIConfiguration": {
    "DashboardUrl": "https://at-pas.apprenticeships.education.gov.uk/"
  },
  "ProviderIdams": {
    "MetadataAddress": "https://adfs.preprod.skillsfunding.service.gov.uk/FederationMetadata/2007-06/FederationMetadata.xml",
    "Wtrealm": "https://localhost:5011/"
  }
}
```  
  
You will also need to setup the roatp outer api and have it running (see https://github.com/SkillsFundingAgency/das-apim-endpoints/ and go to the section for 'Course Management')

Open the solution with Visual Studio, and run the project SFA.DAS.Roatp.CourseManagement.Web, running under process 'SFA.DAS.Roatp.CourseManagement.Web' (not IIS)

## Technologies
* .Net 8.0
* NUnit
* Moq
* FluentAssertions


  