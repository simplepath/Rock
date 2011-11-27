//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by the T4\Model.tt template.
//
//     Changes to this file will be lost when the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
//
// THIS WORK IS LICENSED UNDER A CREATIVE COMMONS ATTRIBUTION-NONCOMMERCIAL-
// SHAREALIKE 3.0 UNPORTED LICENSE:
// http://creativecommons.org/licenses/by-nc-sa/3.0/
//
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Rock.EntityFramework
{
	/// <summary>
	/// Entity Framework Context
	/// </summary>
    public partial class RockContext : DbContext
    {
        /// <summary>
        /// Gets or sets the Auths.
        /// </summary>
        /// <value>
        /// the Auths.
        /// </value>
        public DbSet<Rock.Models.Cms.Auth> Auths { get; set; }

        /// <summary>
        /// Gets or sets the Blocks.
        /// </summary>
        /// <value>
        /// the Blocks.
        /// </value>
        public DbSet<Rock.Models.Cms.Block> Blocks { get; set; }

        /// <summary>
        /// Gets or sets the Block Instances.
        /// </summary>
        /// <value>
        /// the Block Instances.
        /// </value>
        public DbSet<Rock.Models.Cms.BlockInstance> BlockInstances { get; set; }

        /// <summary>
        /// Gets or sets the Blogs.
        /// </summary>
        /// <value>
        /// the Blogs.
        /// </value>
        public DbSet<Rock.Models.Cms.Blog> Blogs { get; set; }

        /// <summary>
        /// Gets or sets the Blog Categories.
        /// </summary>
        /// <value>
        /// the Blog Categories.
        /// </value>
        public DbSet<Rock.Models.Cms.BlogCategory> BlogCategories { get; set; }

        /// <summary>
        /// Gets or sets the Blog Posts.
        /// </summary>
        /// <value>
        /// the Blog Posts.
        /// </value>
        public DbSet<Rock.Models.Cms.BlogPost> BlogPosts { get; set; }

        /// <summary>
        /// Gets or sets the Blog Post Comments.
        /// </summary>
        /// <value>
        /// the Blog Post Comments.
        /// </value>
        public DbSet<Rock.Models.Cms.BlogPostComment> BlogPostComments { get; set; }

        /// <summary>
        /// Gets or sets the Blog Tags.
        /// </summary>
        /// <value>
        /// the Blog Tags.
        /// </value>
        public DbSet<Rock.Models.Cms.BlogTag> BlogTags { get; set; }

        /// <summary>
        /// Gets or sets the Files.
        /// </summary>
        /// <value>
        /// the Files.
        /// </value>
        public DbSet<Rock.Models.Cms.File> Files { get; set; }

        /// <summary>
        /// Gets or sets the Html Contents.
        /// </summary>
        /// <value>
        /// the Html Contents.
        /// </value>
        public DbSet<Rock.Models.Cms.HtmlContent> HtmlContents { get; set; }

        /// <summary>
        /// Gets or sets the Pages.
        /// </summary>
        /// <value>
        /// the Pages.
        /// </value>
        public DbSet<Rock.Models.Cms.Page> Pages { get; set; }

        /// <summary>
        /// Gets or sets the Page Routes.
        /// </summary>
        /// <value>
        /// the Page Routes.
        /// </value>
        public DbSet<Rock.Models.Cms.PageRoute> PageRoutes { get; set; }

        /// <summary>
        /// Gets or sets the Sites.
        /// </summary>
        /// <value>
        /// the Sites.
        /// </value>
        public DbSet<Rock.Models.Cms.Site> Sites { get; set; }

        /// <summary>
        /// Gets or sets the Site Domains.
        /// </summary>
        /// <value>
        /// the Site Domains.
        /// </value>
        public DbSet<Rock.Models.Cms.SiteDomain> SiteDomains { get; set; }

        /// <summary>
        /// Gets or sets the Users.
        /// </summary>
        /// <value>
        /// the Users.
        /// </value>
        public DbSet<Rock.Models.Cms.User> Users { get; set; }

        /// <summary>
        /// Gets or sets the Attributes.
        /// </summary>
        /// <value>
        /// the Attributes.
        /// </value>
        public DbSet<Rock.Models.Core.Attribute> Attributes { get; set; }

        /// <summary>
        /// Gets or sets the Attribute Qualifiers.
        /// </summary>
        /// <value>
        /// the Attribute Qualifiers.
        /// </value>
        public DbSet<Rock.Models.Core.AttributeQualifier> AttributeQualifiers { get; set; }

        /// <summary>
        /// Gets or sets the Attribute Values.
        /// </summary>
        /// <value>
        /// the Attribute Values.
        /// </value>
        public DbSet<Rock.Models.Core.AttributeValue> AttributeValues { get; set; }

        /// <summary>
        /// Gets or sets the Defined Types.
        /// </summary>
        /// <value>
        /// the Defined Types.
        /// </value>
        public DbSet<Rock.Models.Core.DefinedType> DefinedTypes { get; set; }

        /// <summary>
        /// Gets or sets the Defined Values.
        /// </summary>
        /// <value>
        /// the Defined Values.
        /// </value>
        public DbSet<Rock.Models.Core.DefinedValue> DefinedValues { get; set; }

        /// <summary>
        /// Gets or sets the Entity Changes.
        /// </summary>
        /// <value>
        /// the Entity Changes.
        /// </value>
        public DbSet<Rock.Models.Core.EntityChange> EntityChanges { get; set; }

        /// <summary>
        /// Gets or sets the Field Types.
        /// </summary>
        /// <value>
        /// the Field Types.
        /// </value>
        public DbSet<Rock.Models.Core.FieldType> FieldTypes { get; set; }

        /// <summary>
        /// Gets or sets the Service Logs.
        /// </summary>
        /// <value>
        /// the Service Logs.
        /// </value>
        public DbSet<Rock.Models.Core.ServiceLog> ServiceLogs { get; set; }

        /// <summary>
        /// Gets or sets the Addresses.
        /// </summary>
        /// <value>
        /// the Addresses.
        /// </value>
        public DbSet<Rock.Models.Crm.Address> Addresses { get; set; }

        /// <summary>
        /// Gets or sets the People.
        /// </summary>
        /// <value>
        /// the People.
        /// </value>
        public DbSet<Rock.Models.Crm.Person> People { get; set; }

        /// <summary>
        /// Gets or sets the Phone Numbers.
        /// </summary>
        /// <value>
        /// the Phone Numbers.
        /// </value>
        public DbSet<Rock.Models.Crm.PhoneNumber> PhoneNumbers { get; set; }

        /// <summary>
        /// Gets or sets the Groups.
        /// </summary>
        /// <value>
        /// the Groups.
        /// </value>
        public DbSet<Rock.Models.Groups.Group> Groups { get; set; }

        /// <summary>
        /// Gets or sets the Group Roles.
        /// </summary>
        /// <value>
        /// the Group Roles.
        /// </value>
        public DbSet<Rock.Models.Groups.GroupRole> GroupRoles { get; set; }

        /// <summary>
        /// Gets or sets the Group Types.
        /// </summary>
        /// <value>
        /// the Group Types.
        /// </value>
        public DbSet<Rock.Models.Groups.GroupType> GroupTypes { get; set; }

        /// <summary>
        /// Gets or sets the Members.
        /// </summary>
        /// <value>
        /// the Members.
        /// </value>
        public DbSet<Rock.Models.Groups.Member> Members { get; set; }

        /// <summary>
        /// Gets or sets the Jobs.
        /// </summary>
        /// <value>
        /// the Jobs.
        /// </value>
        public DbSet<Rock.Models.Util.Job> Jobs { get; set; }


        /// <summary>
        /// This method is called when the context has been initialized, but
        /// before the model has been locked down and used to initialize the context. 
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context being created.</param>
        protected override void OnModelCreating( DbModelBuilder modelBuilder )
        {
            modelBuilder.Configurations.Add( new Rock.Models.Cms.AuthConfiguration() );
            modelBuilder.Configurations.Add( new Rock.Models.Cms.BlockConfiguration() );
            modelBuilder.Configurations.Add( new Rock.Models.Cms.BlockInstanceConfiguration() );
            modelBuilder.Configurations.Add( new Rock.Models.Cms.BlogConfiguration() );
            modelBuilder.Configurations.Add( new Rock.Models.Cms.BlogCategoryConfiguration() );
            modelBuilder.Configurations.Add( new Rock.Models.Cms.BlogPostConfiguration() );
            modelBuilder.Configurations.Add( new Rock.Models.Cms.BlogPostCommentConfiguration() );
            modelBuilder.Configurations.Add( new Rock.Models.Cms.BlogTagConfiguration() );
            modelBuilder.Configurations.Add( new Rock.Models.Cms.FileConfiguration() );
            modelBuilder.Configurations.Add( new Rock.Models.Cms.HtmlContentConfiguration() );
            modelBuilder.Configurations.Add( new Rock.Models.Cms.PageConfiguration() );
            modelBuilder.Configurations.Add( new Rock.Models.Cms.PageRouteConfiguration() );
            modelBuilder.Configurations.Add( new Rock.Models.Cms.SiteConfiguration() );
            modelBuilder.Configurations.Add( new Rock.Models.Cms.SiteDomainConfiguration() );
            modelBuilder.Configurations.Add( new Rock.Models.Cms.UserConfiguration() );
            modelBuilder.Configurations.Add( new Rock.Models.Core.AttributeConfiguration() );
            modelBuilder.Configurations.Add( new Rock.Models.Core.AttributeQualifierConfiguration() );
            modelBuilder.Configurations.Add( new Rock.Models.Core.AttributeValueConfiguration() );
            modelBuilder.Configurations.Add( new Rock.Models.Core.DefinedTypeConfiguration() );
            modelBuilder.Configurations.Add( new Rock.Models.Core.DefinedValueConfiguration() );
            modelBuilder.Configurations.Add( new Rock.Models.Core.EntityChangeConfiguration() );
            modelBuilder.Configurations.Add( new Rock.Models.Core.FieldTypeConfiguration() );
            modelBuilder.Configurations.Add( new Rock.Models.Core.ServiceLogConfiguration() );
            modelBuilder.Configurations.Add( new Rock.Models.Crm.AddressConfiguration() );
            modelBuilder.Configurations.Add( new Rock.Models.Crm.PersonConfiguration() );
            modelBuilder.Configurations.Add( new Rock.Models.Crm.PhoneNumberConfiguration() );
            modelBuilder.Configurations.Add( new Rock.Models.Groups.GroupConfiguration() );
            modelBuilder.Configurations.Add( new Rock.Models.Groups.GroupRoleConfiguration() );
            modelBuilder.Configurations.Add( new Rock.Models.Groups.GroupTypeConfiguration() );
            modelBuilder.Configurations.Add( new Rock.Models.Groups.MemberConfiguration() );
            modelBuilder.Configurations.Add( new Rock.Models.Util.JobConfiguration() );
		}
    }
}

