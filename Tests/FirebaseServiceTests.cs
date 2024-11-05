using EasyPaperWork.Services;
using Google.Cloud.Firestore;
using Moq;
using Xunit;

namespace EasyPaperWork.Tests
{
    public class FirebaseServiceTests
    {
        private readonly Mock<FirestoreDb> _firestoreDbMock;
        private readonly FirebaseService _firebaseService;

        public FirebaseServiceTests()
        {
            _firestoreDbMock = new Mock<FirestoreDb>();
            _firebaseService = new FirebaseService();
        }

        [Fact]
        public async Task AdicionarDocumentoAsync_ShouldThrowArgumentException_WhenCollectionIsNull()
        {
            // Arrange
            string colecao = null;
            string documentoId = "testDoc";
            var dados = new Dictionary<string, object> { { "key", "value" } };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _firebaseService.AdicionarDocumentoAsync(colecao, documentoId, dados));
        }

        [Fact]
        public async Task AdicionarDocumentoAsync_ShouldThrowArgumentException_WhenDocumentIdIsNull()
        {
            // Arrange
            string colecao = "testCollection";
            string documentoId = null;
            var dados = new Dictionary<string, object> { { "key", "value" } };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _firebaseService.AdicionarDocumentoAsync(colecao, documentoId, dados));
        }

        [Fact]
        public async Task AdicionarDocumentoAsync_ShouldThrowArgumentException_WhenDataIsNull()
        {
            // Arrange
            string colecao = "testCollection";
            string documentoId = "testDoc";
            Dictionary<string, object> dados = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _firebaseService.AdicionarDocumentoAsync(colecao, documentoId, dados));
        }



    }
}
