## ⛔Never push sensitive information such as client id's, secrets or keys into repositories including in the README file⛔

##  RoATP Course Management Web

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_apis/build/status%2FApprenticeships%20Providers%2Fdas-roatp-coursemanagement-web?repoName=SkillsFundingAgency%2Fdas-roatp-coursemanagement-web&branchName=main)](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_build/latest?definitionId=2826&repoName=SkillsFundingAgency%2Fdas-roatp-coursemanagement-web&branchName=main)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SkillsFundingAgency_das-roatp-coursemanagement-web&metric=alert_status)](https://sonarcloud.io/dashboard?id=SkillsFundingAgency_das-roatp-coursemanagement-web)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)


## About

The front end for providers to view and administer the courses they offer, and the venues or subregions where the courses are offered, and the provider description.

It interacts with an outer api, which in turn interacts with several other sources of data, but most significantly the inner api for roatp data.


## 🚀 Installation

### Pre-Requisites
* A clone of this repository
* Visual Studio or similar IDE
* A storage emulator (for example Azurite)

### Dependencies

* Roatp V2 Api: https://github.com/SkillsFundingAgency/das-roatp-api
* Course Management Outer Api: https://github.com/SkillsFundingAgency/das-apim-endpoints/tree/master/src/RoatpCourseManagement
* Location Api: https://github.com/SkillsFundingAgency/das-location-api

### Config

* Create a Configuration table in your (Development) local storage account.
* Obtain the local config json from the das-employer-config for das-roatp-coursemanagement-web repo (https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-roatp-course-management-web/SFA.DAS.Roatp.CourseManagement.Web.json) 
  * PartitionKey: LOCAL
  * RowKey: SFA.DAS.Roatp.CourseManagement.Web_1.0
  * Data: {The contents of the local config json file}
  
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

## Technologies
* .Net 10.0
* NUnit
* Moq
* FluentAssertions