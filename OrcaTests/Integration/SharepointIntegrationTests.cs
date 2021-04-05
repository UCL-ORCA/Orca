using Microsoft.Extensions.Options;
using Microsoft.SharePoint.Client;
using Orca.Entities;
using Orca.Services;
using Orca.Tools;
using OrcaTests.Services;
using OrcaTests.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OrcaTests.Integration
{
    /// <summary>
    /// Tests in this class require the following environment variables to run:
    /// ORCA_INTEGRATION = TRUE
    /// SHAREPOINT_URL
    /// SHAREPOINT_CLIENT_ID
    /// SHAREPOINT_CLIENT_SECRET
    /// </summary>
    public class SharepointIntegrationTests : IDisposable
    {
        private string _listNameForTest;

        // Test functions(methods) in SharepointManager.cs (namespace Orca.Tools)
        // AddItemToList();
        // GetItemsFromList();
        // CheckListExists();
        // CreateList();
        // -- create list including the list creation and permission modification.

        public SharepointIntegrationTests()
        {
            // generate random list name to avoid collisions.
            _listNameForTest = "SharepointIntegrationTest-" + Guid.NewGuid();
        }

        // Testing for this function need the env variable ORCA_INTEGRATION = TRUE.
        [IntegrationFact]
        public async void CreateListAndAddOrGetItemsTest()
        {
            var sharepointManager = new SharepointManager(Options.Create(SharepointSettingsFromEnv()));
            var listToCreate = _listNameForTest;
            var description = "Sharepoint integration test 1 simple list.";
            string FIELD_XML_SCHEMA = $"<Field DisplayName='Default' Type='Text' Required='TRUE' />";

            bool listExistsBeforeCreating = sharepointManager.CheckListExists(listToCreate);
            Assert.False(listExistsBeforeCreating);

            sharepointManager.CreateList(listToCreate, description, new List<string> { FIELD_XML_SCHEMA });
            
            bool listExistsAfterCreating = sharepointManager.CheckListExists(listToCreate);
            Assert.True(listExistsAfterCreating);

            SharepointListItem testItem = new SharepointListItem();
            testItem["Default"] = "testItem";

            var listBeforeAdding = await sharepointManager.GetItemsFromList(listToCreate);
            int numOfItemsBeforeAdding = listBeforeAdding.Count;
            Assert.Equal(0, numOfItemsBeforeAdding);

            var addItem = await sharepointManager.AddItemToList(listToCreate, testItem);
            bool addItemSuccessful = addItem;
            // Add item to list should be successful.
            Assert.True(addItemSuccessful);
            if (addItemSuccessful)
            {
                var listAfterAdding = await sharepointManager.GetItemsFromList(listToCreate);
                int numOfItemsAfterAdding = listAfterAdding.Count;
                Assert.Equal(1, numOfItemsAfterAdding);
                
                SharepointListItem item = listAfterAdding.First();
                bool itemContainsExpectedKey = item.Keys.Contains("Default");
                bool itemContainsExpectedValue = item.Values.Contains("testItem");
                Assert.True(itemContainsExpectedKey && itemContainsExpectedValue);
                Assert.Equal("testItem", item["Default"]);
            }
        }

        [IntegrationFact]
        public void CreateListAndCheckRoleAssignmentsTest()
        {
            var sharepointManager = new SharepointManager(Options.Create(SharepointSettingsFromEnv()));
            var listToCreate = _listNameForTest;
            var description = "Sharepoint integration test 2 simple list.";
            string FIELD_XML_SCHEMA = $"<Field DisplayName='Default' Type='Text' Required='TRUE' />";

            sharepointManager.CreateList(listToCreate, description, new List<string> { FIELD_XML_SCHEMA });
            
            var spSettings = SharepointSettingsFromEnv();
            using (var _authenticationManager = new PnP.Framework.AuthenticationManager())
            {
                using (var context = _authenticationManager.GetACSAppOnlyContext(spSettings.SharepointUrl, spSettings.ClientId, spSettings.ClientSecret))
                {
                    var orcaSite = context.Web;
                    
                    Microsoft.SharePoint.Client.List createdList = orcaSite.Lists.GetByTitle(listToCreate);
                    
                    context.Load(createdList, x => x.HasUniqueRoleAssignments, y => y.RoleAssignments.Include(r => r.Member));
                    context.ExecuteQuery();
                    context.Load(orcaSite.AssociatedOwnerGroup, g => g.Id);
                    context.ExecuteQuery();
                    
                    bool isUniqueRoleAssignments = createdList.HasUniqueRoleAssignments;
                    Assert.True(isUniqueRoleAssignments);

                    int roleAssignmentsCount = createdList.RoleAssignments.Count;
                    Assert.Equal(1, roleAssignmentsCount);
                    
                    var siteOwnerGroupId = orcaSite.AssociatedOwnerGroup.Id;
                    foreach (var assignment in createdList.RoleAssignments)
                    {
                        var id = assignment.Member.Id;
                        Assert.Equal(siteOwnerGroupId, id);
                    }
                }
            }
        }

        [IntegrationFact]
        public async void AddItemToListButFailedTest()
        {
            var sharepointManager = new SharepointManager(Options.Create(SharepointSettingsFromEnv()));
            var listToCreate = _listNameForTest;

            bool listExists = sharepointManager.CheckListExists(listToCreate);
            Assert.False(listExists);

            SharepointListItem testItem = new SharepointListItem();
            testItem["Default"] = "testItem";

            var addItem = await sharepointManager.AddItemToList(listToCreate, testItem);
            bool addItemSuccessful = addItem;
            Assert.False(addItemSuccessful);
        }

        private static SharepointSettings SharepointSettingsFromEnv()
        {
            var settings = new SharepointSettings
            {
                SharepointUrl = Environment.GetEnvironmentVariable("SHAREPOINT_URL"),
                ClientId = Environment.GetEnvironmentVariable("SHAREPOINT_CLIENT_ID"),
                ClientSecret = Environment.GetEnvironmentVariable("SHAREPOINT_CLIENT_SECRET"),
                CourseCatalogListName = "defaultName"
                // No need for course catalog for this class' testing.
                
            };
            return settings;

        }

        /// <summary>
        /// Delete sharepoint list after each test if it was created.
        /// </summary>
        public void Dispose()
        {
            var spSettings = SharepointSettingsFromEnv();

            using (var _authenticationManager = new PnP.Framework.AuthenticationManager())
            {
                using (var context = _authenticationManager.GetACSAppOnlyContext(spSettings.SharepointUrl, spSettings.ClientId, spSettings.ClientSecret))
                {
                    var orcaSite = context.Web;
                    if (orcaSite.ListExists(_listNameForTest))
                    {
                        var list = orcaSite.Lists.GetByTitle(_listNameForTest);
                        list.DeleteObject();
                        orcaSite.DeleteNavigationNode(_listNameForTest, string.Empty, PnP.Framework.Enums.NavigationType.QuickLaunch);
                        context.ExecuteQuery();
                    }
                }
            }
        }
    }
}
