using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Dev
{
    public class EntryPoint : IInitializable
    {
        private NetworkRunner _networkRunner;
        private NetworkCallbacks _networkCallbacks;
        private NetworkSceneManagerDefault _networkSceneManagerDefault;

        public EntryPoint(NetworkCallbacks networkCallbacks, NetworkRunner networkRunner, NetworkSceneManagerDefault networkSceneManagerDefault)
        {
            _networkSceneManagerDefault = networkSceneManagerDefault;
            _networkRunner = networkRunner;
            _networkCallbacks = networkCallbacks;
        }

        public void Initialize()
        {
            StartGame();
        }

        private void JoinLobby()
        {

          //  _networkRunner.JoinSessionLobby((SessionLobby.ClientServer));

        }
        
        private void StartGame()
        {
            _networkRunner.AddCallbacks(_networkCallbacks);

            var startGameArgs = new StartGameArgs();

            startGameArgs.GameMode = GameMode.AutoHostOrClient;
            startGameArgs.SessionName = "Test_Game2";
            startGameArgs.Scene = SceneManager.GetActiveScene().buildIndex;
            startGameArgs.SceneManager = _networkSceneManagerDefault;
            
            _networkRunner.StartGame(startGameArgs);
        }
    }
}