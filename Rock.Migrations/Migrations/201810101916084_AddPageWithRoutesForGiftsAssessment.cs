// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
namespace Rock.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    /// <summary>
    ///
    /// </summary>
    public partial class AddPageWithRoutesForGiftsAssessment : Rock.Migrations.RockMigration
    {
        /// <summary>
        /// Operations to be performed during the upgrade process.
        /// </summary>
        public override void Up()
        {
            AddPageWithRoutsForGiftAssessment();
        }
        
        /// <summary>
        /// Operations to be performed during the downgrade process.
        /// </summary>
        public override void Down()
        {
        }

        private void AddPageWithRoutsForGiftAssessment()
        {
            RockMigrationHelper.AddPage( true, "EBAA5140-4B8F-44B8-B1E8-C73B654E4B22", "5FEAF34C-7FB6-4A11-8A1E-C452EC7849BD", "Gifts Assessment", "", "06410598-3DA4-4710-A047-A518157753AB", "" ); // Site:External Website
            RockMigrationHelper.AddPageRoute( "06410598-3DA4-4710-A047-A518157753AB", "GiftsAssessment", "1B580CA3-F1DB-443F-ABA4-F9C7EC6A8A1B" );// for Page:Gifts Assessment
            RockMigrationHelper.AddPageRoute( "06410598-3DA4-4710-A047-A518157753AB", "GiftsAssessment/{rckipid}", "B991B18C-9B71-4BA9-8149-760CF15F37F3" );// for Page:Gifts Assessment
            RockMigrationHelper.UpdateBlockType( "Gifts Assessment", "Allows you to take a spiritual gifts test and saves your spiritual gifts score.", "~/Blocks/Crm/GiftsAssessment.ascx", "CRM", "A7E86792-F0ED-46F2-988D-25EBFCD1DC96" );
            RockMigrationHelper.AddBlock( true, "06410598-3DA4-4710-A047-A518157753AB".AsGuid(), null, "F3F82256-2D66-432B-9D67-3552CD2F4C2B".AsGuid(), "A7E86792-F0ED-46F2-988D-25EBFCD1DC96".AsGuid(), "Gifts Assessment", "Main", @"", @"", 0, "B76F0F54-E03A-4835-A69D-B6D6F6499D4A" );
            RockMigrationHelper.UpdateBlockTypeAttribute( "A7E86792-F0ED-46F2-988D-25EBFCD1DC96", "1D0D3794-C210-48A8-8C68-3FBEC08A6BA5", "Results Message", "ResultsMessage", "", @"The text (HTML) to display at the top of the results section.<span class='tip tip-lava'></span><span class='tip tip-html'></span>", 0, @"  <div class='row'>      <div class='col-md-12'>      <h2 class='h2'> Dominant Gifts</h2>      </div>      <div class='col-md-9'>      <table class='table table-bordered table-responsive'>      <thead>          <tr>              <th>                  Spiritual Gift              </th>              <th>                  You are uniquely wired to:              </th>          </tr>      </thead>      <tbody>          {% for dominantGift in DominantGifts %}          <tr>          <td>          {{ dominantGift.Value }}          </td>          <td>          {{ dominantGift.Description }}              </td>          </tr>          {% endfor %}      </tbody>      </table>      </div>      </div>        <div class='row'>      <div class='col-md-12'>          <h2 class='h2'> Supportive Gifts</h2>      </div>      <div class='col-md-9'>          <table class='table table-bordered table-responsive'>              <thead>                  <tr>                     <th>                      Spiritual Gift                      </th>                      <th>                      You are uniquely wired to:                      </th>                  </tr>              </thead>              <tbody>              {% for supportiveGift in SupportiveGifts %}              <tr>                  <td>                  {{ supportiveGift.Value }}                  </td>                  <td>                  {{ supportiveGift.Description }}                  </td>              </tr>                  {% endfor %}              </tbody>          </table>      </div>  </div?  <div class='row'>      <div class='col-md-12'>          <h2 class='h2'> Other Gifts</h2>      </div>      <div class='col-md-9'>          <table class='table table-bordered table-responsive'>              <thead>                  <tr>                     <th>                      Spiritual Gift                      </th>                      <th>                      You are uniquely wired to:                      </th>                  </tr>              </thead>              <tbody>                  {% for otherGift in OtherGifts %}              <tr>                  <td>                  {{ otherGift.Value }}                  </td>                  <td>                      {{ otherGift.Description }}                  </td>              </tr>                  {% endfor %}             </tbody>          </table>      </div>  </div>  ", "85256610-56EB-4E6F-B62B-A5517B54B39E" );
            RockMigrationHelper.UpdateBlockTypeAttribute( "A7E86792-F0ED-46F2-988D-25EBFCD1DC96", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Set Page Icon", "SetPageIcon", "", @"The css class name to use for the heading icon.", 1, @"fa fa-gift", "DA7752F5-9F21-4391-97F3-BB7D35F885CE" );
            RockMigrationHelper.UpdateBlockTypeAttribute( "A7E86792-F0ED-46F2-988D-25EBFCD1DC96", "9C204CD0-1233-41C5-818A-C5DA439445AA", "Set Page Title", "SetPageTitle", "", @"The text to display as the heading.", 0, @"Spiritual Gifts Assessment", "85107259-0A30-4F1A-A651-CBED5243B922" );
            RockMigrationHelper.UpdateBlockTypeAttribute( "A7E86792-F0ED-46F2-988D-25EBFCD1DC96", "1EDAFDED-DFE6-4334-B019-6EECBA89E05A", "Allow Retakes", "AllowRetakes", "", @"If enabled, the person can retake the test after the minimum days passes.", 3, @"True", "9DC69746-7AD4-4BC9-B6EE-27E24774CE5B" );
            RockMigrationHelper.UpdateBlockTypeAttribute( "A7E86792-F0ED-46F2-988D-25EBFCD1DC96", "1D0D3794-C210-48A8-8C68-3FBEC08A6BA5", "Instructions", "Instructions", "", @"The text (HTML) to display at the top of the instructions section.  <span class='tip tip-lava'></span> <span class='tip tip-html'></span>", 0, @"              <h2>Welcome to Your Spiritual Gifts Assessment</h2>              <p>{{ Person.NickName }}, the purpose of this assessment is to help you identify spiritual gifts that are most naturally used in the life of the local church. This survey does not include all spiritual gifts, just those that are often seen in action for most churches and most people.</p>              <p>In churches it’s not uncommon to see 90% of the work being done by a weary 10%. Why does this happen? Partially due to ignorance and partially due to avoidance of spiritual gifts. Here’s the process:</p>              <ol><li>Discover the primary gifts given to us at our Spiritual birth.</li>              <li>Learn what these gifts are and what they are not.</li>              <li>See where these gifts fit into the functioning of the body. </li>              </ol>              <p>When you are working within your Spirit-given gifts, you will be most effective for the body of Christ in your local setting. </p> <p>     Before you begin, please take a moment and pray that the Holy Spirit would guide your thoughts, calm your mind, and help you respond to each item as honestly as you can. Don't spend much time on each item. Your first instinct is probably your best response.</p>", "86C9E794-B678-4453-A831-FE348A440646" );
            RockMigrationHelper.UpdateBlockTypeAttribute( "A7E86792-F0ED-46F2-988D-25EBFCD1DC96", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Number of Questions", "NumberofQuestions", "", @"The number of questions to show per page while taking the test", 2, @"17", "861F4601-82B7-46E3-967F-2E03D769E2D2" );
            RockMigrationHelper.UpdateBlockTypeAttribute( "A7E86792-F0ED-46F2-988D-25EBFCD1DC96", "A75DFC58-7A1B-4799-BF31-451B2BBE38FF", "Min Days To Retake", "MinDaysToRetake", "", @"The number of days that must pass before the test can be taken again.", 4, @"360", "44272FB2-27DC-452D-8BBB-2F76266FA92E" );
        }
    }
}
