# FruitsApi
 A powerful Web api which provides the properties of a fruit. It collects the data from an image and returns the properties.You can use this api to easily identify a particular fruit.
 ## Application
 -  Azure Computer Vision
 -  FruityVice
 -  Image of a fruit
 -  Swagger UI
 -  Azure Web App
 -  Azure Blob Storage
 
 ## Packages
- Microsoft.AspNetCore.Mvc
- using Newtonsoft.Json.Linq
- SwashBuckle
- Azure Blob SDK

## Documentation
The API consists of one function.The function is used to upload an image for processing. 
[Swagger Documentation]("https://fruitapi.azurewebsites.net/index.html")
#### On Local
- POST https://musicplatform.azurewebsites.net/api/fruits

#### How it works
- An image is uploaded using the post method, the image is converted into byte data and sent to the computer vision to interpret. The computer vision sends back the data , the data is processed and sent to the open source FruityVice to return the properties.
 
###### This project is open for contribution.Fork and pull requests.
## Thank you.
 
