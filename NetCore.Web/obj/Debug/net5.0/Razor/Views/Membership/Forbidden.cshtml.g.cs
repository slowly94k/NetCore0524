#pragma checksum "C:\hwfile\vs2019\z0516\NetCore\NetCore.Web\Views\Membership\Forbidden.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "da3566f7cd8c72837a53619832d3c576bf870414"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Membership_Forbidden), @"mvc.1.0.view", @"/Views/Membership/Forbidden.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\hwfile\vs2019\z0516\NetCore\NetCore.Web\Views\_ViewImports.cshtml"
using NetCore.Web;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\hwfile\vs2019\z0516\NetCore\NetCore.Web\Views\_ViewImports.cshtml"
using NetCore.Web.Models;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\hwfile\vs2019\z0516\NetCore\NetCore.Web\Views\_ViewImports.cshtml"
using NetCore.Data.ViewModels;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"da3566f7cd8c72837a53619832d3c576bf870414", @"/Views/Membership/Forbidden.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"59ea6c56e5655097cd1a119e9f07c967221b761d", @"/Views/_ViewImports.cshtml")]
    #nullable restore
    public class Views_Membership_Forbidden : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    #nullable disable
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#nullable restore
#line 2 "C:\hwfile\vs2019\z0516\NetCore\NetCore.Web\Views\Membership\Forbidden.cshtml"
   ViewData["Title"] = "접근권한 불가입니다.";
                Layout = "~/Views/Shared/_Layout.cshtml"; 

#line default
#line hidden
#nullable disable
            WriteLiteral("\n<h2>");
#nullable restore
#line 5 "C:\hwfile\vs2019\z0516\NetCore\NetCore.Web\Views\Membership\Forbidden.cshtml"
Write(ViewData["Title"]);

#line default
#line hidden
#nullable disable
            WriteLiteral("</h2>\n\n<div class=\"text-danger\">");
#nullable restore
#line 7 "C:\hwfile\vs2019\z0516\NetCore\NetCore.Web\Views\Membership\Forbidden.cshtml"
                    Write(Html.Raw(ViewData["Message"]));

#line default
#line hidden
#nullable disable
            WriteLiteral("</div>");
        }
        #pragma warning restore 1998
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; } = default!;
        #nullable disable
        #nullable restore
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; } = default!;
        #nullable disable
    }
}
#pragma warning restore 1591
