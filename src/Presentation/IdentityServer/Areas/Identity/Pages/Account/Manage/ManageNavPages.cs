﻿using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdentityServer.Areas.Identity.Pages.Account.Manage;

public static class ManageNavPages
{
    #region Public Properties

    public static string ChangePassword => "ChangePassword";

    public static string DeletePersonalData => "DeletePersonalData";

    public static string DownloadPersonalData => "DownloadPersonalData";

    public static string Email => "Email";

    public static string ExternalLogins => "ExternalLogins";

    public static string Index => "Index";

    public static string PersonalData => "PersonalData";

    public static string TwoFactorAuthentication => "TwoFactorAuthentication";

    #endregion Public Properties

    #region Public Methods

    public static string ChangePasswordNavClass(ViewContext viewContext) => PageNavClass(viewContext, ChangePassword);

    public static string DeletePersonalDataNavClass(ViewContext viewContext) => PageNavClass(viewContext, DeletePersonalData);

    public static string DownloadPersonalDataNavClass(ViewContext viewContext) => PageNavClass(viewContext, DownloadPersonalData);

    public static string EmailNavClass(ViewContext viewContext) => PageNavClass(viewContext, Email);

    public static string ExternalLoginsNavClass(ViewContext viewContext) => PageNavClass(viewContext, ExternalLogins);

    public static string IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);

    public static string PageNavClass(ViewContext viewContext, string page)
    {
        string? activePage = viewContext.ViewData["ActivePage"] as string
            ?? Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
        return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : string.Empty;
    }

    public static string PersonalDataNavClass(ViewContext viewContext) => PageNavClass(viewContext, PersonalData);

    public static string TwoFactorAuthenticationNavClass(ViewContext viewContext) => PageNavClass(viewContext, TwoFactorAuthentication);

    #endregion Public Methods
}