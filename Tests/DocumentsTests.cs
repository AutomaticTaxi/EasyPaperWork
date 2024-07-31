using System;
using System.ComponentModel;
using EasyPaperWork.Models;
using Moq;
using Xunit;

namespace EasyPaperWork.Tests
{
    public class DocumentsTests
    {
    
        [Fact]
        public void SettingName_ShouldTriggerPropertyChanged()
        {
            // Arrange
            var document = new Documents();
            var wasCalled = false;
            document.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(Documents.Name))
                {
                    wasCalled = true;
                }
            };

            // Act
            document.Name = "Test Document";

            // Assert
            Assert.True(wasCalled);
        }

        [Fact]
        public void SettingDocumentType_ShouldTriggerPropertyChanged()
        {
            // Arrange
            var document = new Documents();
            var wasCalled = false;
            document.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(Documents.DocumentType))
                {
                    wasCalled = true;
                }
            };

            // Act
            document.DocumentType = "PDF";

            // Assert
            Assert.True(wasCalled);
        }
    }
}
