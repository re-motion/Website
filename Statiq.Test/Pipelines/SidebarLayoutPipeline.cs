﻿using System.Linq;
using Statiq.Common;
using Statiq.Core;

namespace Statiq.Test.Pipelines
{
  class SidebarLayoutPipeline : Pipeline
  {
    public SidebarLayoutPipeline()
    {
      Dependencies.AddRange(new []{nameof(CreateMainArticlesPipeline), nameof(CreateBlogIndexPipeline)});

      InputModules = new ModuleList
      {
          new ReadFiles("_sidebarlayout.hbs"),
          new SetMetadata("layout", Config.FromContext(async context => await context.Inputs
              .Single()
              .GetContentStringAsync())),
      };

      ProcessModules = new ModuleList
      {
          new SetMetadata("menuitem", Config.FromContext(context => context.Outputs
              .FromPipeline(nameof(CreateMainArticlesPipeline))
              .OrderBy(d => d.Get<int>("index"))
              .Select(d => new
              {
                  url = d.GetLink(),
                  name = d.GetString("menutitle"),
              }))),
          new SetMetadata("blogs", Config.FromContext(context => context.Outputs
              .FromPipeline(nameof(CreateBlogIndexPipeline))
              .Select(d => new
              {
                  url = d.GetLink(),
                  name = d.GetString("category"),
              })))
      };
    }
  }
}
