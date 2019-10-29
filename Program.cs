using NLog;
using BlogsConsole.Models;
using System;
using System.Linq;

namespace BlogsConsole
{
    class MainClass
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static void Main(string[] args)
        {
            logger.Info("Program started");
            try
            {
                String input;
                do
                {
                    Console.WriteLine("1) Display all blogs");
                    Console.WriteLine("2) Add blog");
                    Console.WriteLine("3) Create Post");
                    Console.WriteLine("enter anything else to exit");
                    input = Console.ReadLine();

                    var db = new BloggingContext();
                    var query = db.Blogs.OrderBy(b => b.Name);
                    switch (input)
                    {
                        case "1":
                            // Display all Blogs from the database
                            Console.WriteLine("All blogs in the database:");
                            foreach (var item in query)
                            {
                                Console.WriteLine(item.Name);
                            }
                            break;
                        case "2":
                            // Create and save a new Blog
                            Console.Write("Enter a name for a new Blog: ");
                            var name = Console.ReadLine();

                            //check if blog already exists
                            if (checkBlog(name))
                            {
                                var blog = new Blog { Name = name };
                                db.AddBlog(blog);
                                logger.Info("Blog added - {name}", name);
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Blog already exists");
                                break;
                            }
                        case "3":
                            Console.WriteLine("select the blog you wish to post to");
                            String blogToPostTo = Console.ReadLine();

                            //check that blog exists
                            if (!checkBlog(blogToPostTo))
                            {
                                //get blog
                                var blogItem = db.Blogs.Where(b => b.Name.Equals(blogToPostTo));
                                Blog blog = blogItem.First();

                                //get post details
                                String postTitle = "";
                                while (String.IsNullOrEmpty(postTitle))
                                {
                                    Console.WriteLine("enter a post title");
                                    postTitle = Console.ReadLine();
                                }

                                String postContent = "";
                                while (String.IsNullOrEmpty(postContent))
                                {
                                    Console.WriteLine("Enter content for the post");
                                    postContent = Console.ReadLine();
                                }

                                //make post
                                var post = new Post {Title = postTitle, Content = postContent, BlogId = blog.BlogId};

                                //add post to blogList
                                db.AddPost(post);
                            }
                            else
                            {
                                Console.WriteLine("Blog does not exist");
                            }
                                break;
                    }
                } while (input == "1" || input == "2" || input == "3");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            logger.Info("Program ended");
        }

        public static Boolean checkBlog(String name)
        {
            var unique = true;
            var db = new BloggingContext();
            var query = db.Blogs.OrderBy(b => b.Name);

            foreach (var item in query)
            {
                if (name == item.Name)
                {
                    unique = false;
                }
            }
            return unique;
        }
    }
}
