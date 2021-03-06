﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using System.Web.Http.Results;
using System.Web.OData;
using System.Web.OData.Routing;

namespace OpenTypeSample
{
    public class BooksController : ODataController
    {
        private IList<Book> _books = new List<Book>
        {
            new Book
            {
                ISBN = "978-0-7356-8383-9",
                Title = "SignalR Programming in Microsoft ASP.NET",
                Press = new Press
                {
                    Name = "Microsoft Press",
                    Category = Category.Book
                }
            },

            new Book
            {
                ISBN = "978-0-7356-7942-9",
                Title = "Microsoft Azure SQL Database Step by Step",
                Press = new Press
                {
                    Name = "Microsoft Press",
                    Category = Category.EBook,
                    DynamicProperties = new Dictionary<string, object>
                    {
                        {"Blog", "http://blogs.msdn.com/b/microsoft_press/"},
                        {
                            "Address", new Address
                            {
                                City = "Redmond",
                                Street = "One Microsoft Way"
                            }
                        }
                    }
                },
                DynamicProperties = new Dictionary<string, object>
                {
                    {"Published", new DateTimeOffset(2014, 7, 3, 0, 0, 0, 0, new TimeSpan(0))},
                    {"Authors", new[] {"Leonard G. Lobel", "Eric D. Boyd"}},
                    {"OtherCategories", new[] {Category.Book, Category.Magazine}}
                }
            },

            new Book
            {
                ISBN = "201-0-699",
                Title = "Sam Book1",
                Press = new Press
                {
                    Name = "Microsoft Press",
                    Category = Category.Book
                },
                DynamicProperties = new Dictionary<string, object>
                {
                    {"Sold", 9}
                }
            },

            new Book
            {
                ISBN = "102-9-799",
                Title = "Sam Book2",
                Press = new Press
                {
                    Name = "Microsoft Press",
                    Category = Category.Book
                },
                DynamicProperties = new Dictionary<string, object>
                {
                    {"Sold", 19}
                }
            },
        };

        [EnableQuery]
        public IQueryable<Book> Get()
        {
            return _books.AsQueryable();
        }

        public IHttpActionResult Get([FromODataUri] string key)
        {
            Book book = _books.FirstOrDefault(e => e.ISBN == key);
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }

        public IHttpActionResult Post(Book book)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // For this sample, we aren't enforcing unique keys.
            _books.Add(book);
            return Created(book);
        }

        public IHttpActionResult GetDynamicProperty([FromODataUri] string key, [FromODataUri] string dynamicProperty)
        {
            Book book = _books.FirstOrDefault(e => e.ISBN == key);
            if (book == null)
            {
                return NotFound();
            }

            object value;
            if (!book.DynamicProperties.TryGetValue(dynamicProperty, out value))
            {
                return NotFound();
            }

            return Ok(value, value.GetType());
        }

        [HttpGet]
        [ODataRoute("Books({key})/Press/{pName:dynamicproperty}")]
        public IHttpActionResult XxxxDynamicPropertyXxxx([FromODataUri] string key, [FromODataUri] string pName)
        {
            Book book = _books.FirstOrDefault(e => e.ISBN == key);
            if (book == null)
            {
                return NotFound();
            }

            object value;
            if (!book.Press.DynamicProperties.TryGetValue(pName, out value))
            {
                return NotFound();
            }

            return Ok(value, value.GetType());
        }

        private IHttpActionResult Ok(object content, Type type)
        {
            var resultType = typeof(OkNegotiatedContentResult<>).MakeGenericType(type);
            return Activator.CreateInstance(resultType, content, this) as IHttpActionResult;
        }
    }
}
