// using OnlineBookstore.main.models;
//
// namespace OnlineBookstore.test.api.steps
// {
//     public class BookSteps : BaseSteps
//     {
//         private Book GetAuthorByType(string bookType)
//         {
//             return new Book
//             {
//                 Id = id,
//                 Title = title,
//                 Description = description,
//                 PageCount = pageCount,
//                 Excerpt = excerpt,
//                 PublishDate = publishDate
//                 
//                 
//                 
//                 
//                 Id = _config[$"{bookType}:Id"],
//                 IdBook = _config[$"{bookType}:IdBook"],
//                 FirstName = _config[$"{bookType}:FirstName"],
//                 LastName = _config[$"{bookType}:LastName"]
//             };
//         }
//
//         protected Author existingAuthor => GetAuthorByType("ExistingAuthor");
//         protected Author updateAuthor => GetAuthorByType("UpdateAuthor");
//         protected Author deleteAuthor => GetAuthorByType("DeleteAuthor");
//     }
// }