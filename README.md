# SharePointCore
Open source SharePoint REST API built on C# and .NET Core

KEEPING THIS FOR REFERENCE. I DON'T PLAN ON UPGRADING THIS.

# Tasks

## Base
I will start working my way trough the two typese of SharePoint user authentication. I found a good [blog post](https://blog.sprider.org/2016/09/15/access-sharepoint-online-rest-api-via-google-postman-with-user-context/) that demonstrates this and a [link](https://github.com/sprider/wordpress/tree/master/Samples/SPOL_REST_Test) to a C# example in github that I will port to .NET Core.

## Others
I need to abstract a little the sending of requests with some kind of sender class that accepts options like type of requests and headers. Later steps would be to abstract the whole REST endpoints as classes like list and item.

# Tests
I am still not sure how to test the library properly. Open to suggestions on my mail: capitane71@gmail.com
