using System;
using System.ComponentModel;
using EasyPaperWork.Models;
using Moq;
using Xunit;

namespace EasyPaperWork.Tests
{
    public class UserModelTests
    {
        [Fact]
        public void SettingId_ShouldTriggerPropertyChanged()
        {
            // Arrange
            var userModel = new UserModel();
            var wasCalled = false;
            userModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(UserModel.Id))
                {
                    wasCalled = true;
                }
            };


            // Assert
            Assert.True(wasCalled);
        }

        [Fact]
        public void SettingName_ShouldTriggerPropertyChanged()
        {
            // Arrange
            var userModel = new UserModel();
            var wasCalled = false;
            userModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(UserModel.Name))
                {
                    wasCalled = true;
                }
            };

            // Act
            userModel.Name = "Test Name";

            // Assert
            Assert.True(wasCalled);
        }

        [Fact]
        public void SettingEmail_ShouldTriggerPropertyChanged()
        {
            // Arrange
            var userModel = new UserModel();
            var wasCalled = false;
            userModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(UserModel.Email))
                {
                    wasCalled = true;
                }
            };

            // Act
            userModel.Email = "test@example.com";

            // Assert
            Assert.True(wasCalled);
        }

        [Fact]
        public void SettingPassword_ShouldTriggerPropertyChanged()
        {
            // Arrange
            var userModel = new UserModel();
            var wasCalled = false;
            userModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(UserModel.Passoword))
                {
                    wasCalled = true;
                }
            };

            // Act
            userModel.Passoword = "newpassword";

            // Assert
            Assert.True(wasCalled);
        }

        [Fact]
        public void SettingAccountType_ShouldTriggerPropertyChanged()
        {
            // Arrange
            var userModel = new UserModel();
            var wasCalled = false;
            userModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(UserModel.AccountType))
                {
                    wasCalled = true;
                }
            };

            // Act
            userModel.AccountType = "Premium";

            // Assert
            Assert.True(wasCalled);
        }
    }
}
