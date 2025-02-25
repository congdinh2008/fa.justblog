﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using FA.JustBlog.Data;
using FA.JustBlog.Models.Common;
using FA.JustBlog.Services;
using FA.JustBlog.WebMVC.Areas.Admin.ViewModels;

namespace FA.JustBlog.WebMVC.Areas.Admin.Controllers
{
    [Authorize]
    public class PostManagementController : Controller
    {
        private readonly IPostServices _postServices;
        private readonly ICategoryServices _categoryServices;
        private readonly ITagServices _tagServices;

        public PostManagementController(IPostServices postServices,
            ICategoryServices categoryServices,
            ITagServices tagServices)
        {
            _postServices = postServices;
            _categoryServices = categoryServices;
            _tagServices = tagServices;
        }

        // GET: Admin/PostManagement
        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString,
            int? pageIndex = 1, int pageSize = 2)
        {
            ViewData["CurrentPageSize"] = pageSize;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["TitleSortParm"] = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["UrlSlugSortParm"] = sortOrder == "UrlSlug" ? "urlSlug_desc" : "UrlSlug";
            ViewData["PublishedSortParm"] = sortOrder == "Published" ? "published_desc" : "Published";
            ViewData["PublishedDateSortParm"] = sortOrder == "PublishedDate" ? "publishedDate_desc" : "PublishedDate";
            ViewData["UpdatedAtSortParm"] = sortOrder == "UpdatedAt" ? "updatedAt_desc" : "UpdatedAt";

            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            // x => x.Name.Contains(searchString)
            Expression<Func<Post, bool>> filter = null;

            if (!string.IsNullOrEmpty(searchString))
            {
                filter = p => p.Title.Contains(searchString) || p.ShortDescription.Contains(searchString);
            }

            // q => q.OrderByDescending(c => c.Name)
            Func<IQueryable<Post>, IOrderedQueryable<Post>> orderBy = null;

            switch (sortOrder)
            {
                case "title_desc":
                    orderBy = q => q.OrderByDescending(c => c.Title);
                    break;
                case "UrlSlug":
                    orderBy = q => q.OrderBy(c => c.UrlSlug);
                    break;
                case "urlSlug_desc":
                    orderBy = q => q.OrderByDescending(c => c.UrlSlug);
                    break;
                case "Published":
                    orderBy = q => q.OrderBy(c => c.Published);
                    break;
                case "published_desc":
                    orderBy = q => q.OrderByDescending(c => c.Published);
                    break;
                case "PublishedDate":
                    orderBy = q => q.OrderBy(c => c.PublishedDate);
                    break;
                case "publishedDate_desc":
                    orderBy = q => q.OrderByDescending(c => c.PublishedDate);
                    break;
                case "UpdatedAt":
                    orderBy = q => q.OrderBy(c => c.UpdatedAt);
                    break;
                case "updatedAt_desc":
                    orderBy = q => q.OrderByDescending(c => c.UpdatedAt);
                    break;
                default:
                    orderBy = q => q.OrderBy(c => c.Title);
                    break;
            }

            var categories = await _postServices.GetAsync(filter: filter, orderBy: orderBy, pageIndex: pageIndex ?? 1, pageSize: pageSize);

            return View(categories);
        }


        // GET: Admin/PostManagement/Create
        public ActionResult Create()
        {
            ViewBag.Categories = new SelectList(_categoryServices.GetAll(), "Id", "Name");
            var postViewModel = new PostViewModel();
            postViewModel.Tags = _tagServices.GetAll().Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name });
            return View(postViewModel);
        }

        // POST: Admin/PostManagement/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Create(PostViewModel postViewModel)
        {
            if (ModelState.IsValid)
            {
                var post = new Post
                {
                    Id = Guid.NewGuid(),
                    Title = postViewModel.Title,
                    UrlSlug = postViewModel.UrlSlug,
                    ShortDescription = postViewModel.ShortDescription,
                    ImageUrl = postViewModel.ImageUrl,
                    PostContent = postViewModel.PostContent,
                    Published = postViewModel.Published,
                    CategoryId = postViewModel.CategoryId,
                    Tags = await GetSelectedTagFromIds(postViewModel.SelectedTagIds)
                };
                var result = await _postServices.AddAsync(post);

                return RedirectToAction("Index");
            }

            ViewBag.Categories = new SelectList(await _categoryServices.GetAllAsync(), "Id", "Name", postViewModel.CategoryId);
            postViewModel.Tags = _tagServices.GetAll().Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name });
            return View(postViewModel);
        }

        private async Task<ICollection<Tag>> GetSelectedTagFromIds(IEnumerable<Guid> selectedTagIds)
        {
            var tags = new List<Tag>();

            if (selectedTagIds == null)
            {
                return tags;
            }
            var tagEntities = await _tagServices.GetAllAsync();

            foreach (var item in tagEntities)
            {
                if (selectedTagIds.Any(x => x == item.Id))
                {
                    tags.Add(item);
                }
            }
            return tags;
        }

        // GET: Admin/PostManagement/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {
            
            var post = await _postServices.GetByIdAsync((Guid)id);
            var postViewModel = new PostViewModel()
            {
                Id = post.Id,
                Title = post.Title,
                UrlSlug = post.UrlSlug,
                ShortDescription = post.ShortDescription,
                ImageUrl = post.ImageUrl,
                PostContent = post.PostContent,
                Published = post.Published,
                CategoryId = post.CategoryId,
                SelectedTagIds = post.Tags.Select(x => x.Id)
            };
            ViewBag.Categories = new SelectList(_categoryServices.GetAll(), "Id", "Name", postViewModel.CategoryId);
            postViewModel.Tags = _tagServices.GetAll().Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name });
            ViewBag.TagList = _tagServices.GetAll();
            return View(postViewModel);
        }

        // POST: Admin/PostManagement/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public async Task<ActionResult> Edit(PostViewModel postViewModel)
        {
            if (ModelState.IsValid)
            {
                var post = await _postServices.GetByIdAsync(postViewModel.Id);
                if(post == null)
                {
                    return HttpNotFound();
                }
                post.Title = postViewModel.Title;
                post.UrlSlug = postViewModel.UrlSlug;
                post.ShortDescription = postViewModel.ShortDescription;
                post.ImageUrl = postViewModel.ImageUrl;
                post.PostContent = postViewModel.PostContent;
                post.Published = postViewModel.Published;
                post.CategoryId = postViewModel.CategoryId;
                await UpdateSelectedTagFromIds(postViewModel.SelectedTagIds, post);
                
                var result = await _postServices.UpdateAsync(post);
                if (result)
                {
                    TempData["Message"] = "Update successful!";
                }
                else
                {
                    TempData["Message"] = "Update failed!";

                }
                return RedirectToAction("Index");
            }
            ViewBag.Categories = new SelectList(await _categoryServices.GetAllAsync(), "Id", "Name", postViewModel.CategoryId);
            postViewModel.Tags = _tagServices.GetAll().Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name });
            ViewBag.TagList = _tagServices.GetAll();
            return View(postViewModel);
        }

        private async Task UpdateSelectedTagFromIds(IEnumerable<Guid> selectedTagIds, Post post)
        {
            var tags = post.Tags;
            foreach (var item in tags.ToList())
            {
                post.Tags.Remove(item);
            }
            post.Tags = await GetSelectedTagFromIds(selectedTagIds);
        }

        // POST: Admin/PostManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var result = await _postServices.DeleteAsync(id);
            if (result)
            {
                TempData["Message"] = "Delete Successful";
            }
            else
            {
                TempData["Message"] = "Delete failed";
            }
            return RedirectToAction("Index");
        }
    }
}
