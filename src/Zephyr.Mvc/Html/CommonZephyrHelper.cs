﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Xml.Linq;
using Zephyr.Web.Mvc.Html.Flash;
using Zephyr.Web.Mvc.Html.Models;

namespace Zephyr.Web.Mvc.Html
{
    public static class CommonZephyrlHelper
    {
        public static bool ExistsView<TModel>(this ZephyrHtmlHelper<TModel> zephyrHelper, string viewName) where TModel : class
        {
            ViewEngineResult viewResult =
                ViewEngines.Engines.FindView(zephyrHelper.HtmlHelper.ViewContext.Controller.ControllerContext, viewName,
                                             null);

            return viewResult.View != null;
        }


        public static IHtmlString Flash<TModel>(this ZephyrHtmlHelper<TModel> zephyrHelper, string tagName = "div", bool htmlEncoded = true) where TModel : class
        { 
            //Func<string, XNode> content = message => encoded ? new XText(message) : XElement.Parse(message) as XNode;
            Func<string, XNode> content = message => new XText(message);

            var messages = new FlashStorage(zephyrHelper.HtmlHelper.ViewContext.TempData).Messages.ToList();

            var elements = messages.Select(pair => new XElement(tagName ?? "div", new XAttribute("class", "alert" + " " + pair.Key), 
                new XElement("button", new XAttribute("class", "close"), new XAttribute("data-dismiss", "alert"), "×"),
                content(pair.Value)
                ));
            var html = string.Join(Environment.NewLine, elements.Select(e => e.ToString()));

            return zephyrHelper.HtmlHelper.Raw(htmlEncoded ? html : HttpUtility.HtmlDecode(html));
        }

        /// <summary>
        /// Html helper for generating drop down list for Enum type model property.
        /// </summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>        
        /// <param name="zephyrHtmlHelper">The Zephyr framework html helper accessible through Html.Zephyr() </param>
        /// <param name="expression">The expression.</param>
        /// <param name="defaultText">Default text for empty item on nullable enum type property </param>
        /// <param name="htmlAttributes">The HTML attributes.</param>
        /// <returns></returns>
        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this ZephyrHtmlHelper<TModel> zephyrHtmlHelper, Expression<Func<TModel, TEnum>> expression, string defaultText="", object htmlAttributes=null) where TModel : class
        {
            ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, zephyrHtmlHelper.HtmlHelper.ViewData);
            
            Type enumType = Nullable.GetUnderlyingType(metadata.ModelType) ?? metadata.ModelType;
            
            IEnumerable<TEnum> values = Enum.GetValues(enumType).Cast<TEnum>();

            IList<SelectListItem> items = values.Select(val=>new SelectListItem
                                                            {
                                                                Text = val.ToString(),
                                                                Value = val.ToString(),
                                                                Selected = val.Equals(metadata.Model)
                                                            }).ToList();


            if (metadata.Model is Nullable && metadata.Model == null)
            {
                items.Insert(0, new SelectListItem { Value = "", Text = defaultText, Selected = true });
            }

            // If the enum is nullable, add an 'empty' item to the collection
            if (metadata.IsNullableValueType)
            {
                items.Insert(0, new SelectListItem { Value = "", Text = defaultText });
                //items = _singleEmptyItem.Concat(items).ToList();
            }


            return zephyrHtmlHelper.HtmlHelper.DropDownListFor(expression, items, htmlAttributes);
        }
    }
}