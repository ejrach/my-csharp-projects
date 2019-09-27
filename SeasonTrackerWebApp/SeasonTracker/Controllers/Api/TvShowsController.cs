﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Data.Entity;
using System.Web.Http;
using SeasonTracker.Models;
using SeasonTracker.Dtos;
using AutoMapper;

/*
 * API Controller
 */
namespace SeasonTracker.Controllers.Api
{
    public class TvShowsController : ApiController
    {
        private ApplicationDbContext _context;

        public TvShowsController()
        {
            _context = new ApplicationDbContext();
        }

        // GET /api/tvshows
        // OR
        // GET /api/tvshows?id=5
        //Our typeahead plugin is going to send the query parameter here
        public IEnumerable<TvShowDto> GetTvShows(string query = null)
        {
            //We're applying this query to get the TvShows, after selecting all rows. We don't need
            //to include any related data.
            var tvShowsQuery = _context.TvShows.AsQueryable();

            //If our query has a value, then filter the available tv shows where the name matches the query
            if (!String.IsNullOrWhiteSpace(query))
                tvShowsQuery = tvShowsQuery.Where(t => t.TvShowName.Contains(query));

            return tvShowsQuery
                .ToList()
                .Select(Mapper.Map<TvShow, TvShowDto>);
        }

        //DO NOT DELETE - NEEDED TO COMMENT OUT BECAUSE POSTMAN SAID I HAD TOO MANY GET TVSHOWS ACTIONS
        //We want to return a list of tv shows.
        //This is the convention built into ASP.NET Web API:
        // GET /api/tvshows
        //public IHttpActionResult GetTvShows()
        //{
        //    var tvShowDtos = _context.TvShows
        //        .ToList()
        //        .Select(Mapper.Map<TvShow, TvShowDto>);

        //    return Ok(tvShowDtos);
        //}




        //DO NOT DELETE - NEEDED TO COMMENT OUT BECAUSE POSTMAN SAID I HAD TOO MANY GET TVSHOWS ACTIONS
        //We want to return a single tv show.
        //This will respond to a request like this:
        // GET /api/tvshows/1
        //public IHttpActionResult GetTvShow(int id)
        //{
        //    //get the tv show
        //    var tvShow = _context.TvShows.SingleOrDefault(c => c.Id == id);

        //    //if the tvShow is not found
        //    if (tvShow == null)
        //        return NotFound();

        //    //otherwise return the member
        //    return Ok(Mapper.Map<TvShow, TvShowDto>(tvShow));
        //}

        //To create a tv show, post a tv show to tv shows collection:
        // POST /api/tvshows
        [HttpPost]  //we do this because we are 'creating' a resource, not getting one.
        [Authorize(Roles = RoleName.CanManageTvShows)]
        public IHttpActionResult CreateTvShow(TvShowDto tvShowDto)
        {
            //First we validate the object, and throw exception if model is not valid
            if (!ModelState.IsValid)
                return BadRequest();

            //We need to map the tvShowDto back to our domain model.
            var tvShow = Mapper.Map<TvShowDto, TvShow>(tvShowDto);

            //otherwise we add it to our context and save the changes
            _context.TvShows.Add(tvShow);
            _context.SaveChanges();

            tvShowDto.Id = tvShow.Id;

            //At this point the Id of the tv show will be generated by the server and be returned by the object.
            //As part of RESTful convention, we need to return the URI of the newly created resource to the client.
            //i.e. /api/members/10
            return Created(new Uri(Request.RequestUri + "/" + tvShow.Id), tvShowDto);

        }

        //To update a tvShow:
        // PUT /api/tvshows/1
        [HttpPut]
        [Authorize(Roles = RoleName.CanManageTvShows)]
        public IHttpActionResult UpdateTvShow(int id, TvShowDto tvShowDto)
        {
            //First we validate the object, and throw exception if model is not valid
            if (!ModelState.IsValid)
                return BadRequest();

            var tvShowInDb = _context.TvShows.SingleOrDefault(c => c.Id == id);

            //It's possible that the client sends an invalid Id so we need to check for the existence of the object.
            if (tvShowInDb == null)
                return NotFound();

            Mapper.Map(tvShowDto, tvShowInDb);

            _context.SaveChanges();

            return Ok();
        }

        //To delete a tv show:
        // DELETE /api/tvshows/1
        [HttpDelete]
        [Authorize(Roles = RoleName.CanManageTvShows)]
        public IHttpActionResult DeleteTvShow(int id)
        {
            var tvShowInDb = _context.TvShows.SingleOrDefault(c => c.Id == id);

            //It's possible that the client sends an invalid Id so we need to check for the existence of the object.
            if (tvShowInDb == null)
                return NotFound();

            _context.TvShows.Remove(tvShowInDb);

            _context.SaveChanges();

            return Ok();
        }
    }
}
