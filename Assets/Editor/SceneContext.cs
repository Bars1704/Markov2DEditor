#if UNITY_EDITOR

using System;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PROJ.Levels.Editing
{
    public class SceneContext
    {
        private const string Name = "Editing Context";

        private Scene scene;
        private SceneSetup[] rootScenesSetup;
        private SceneViewCache[] sceneViewCaches;
        private UnityEngine.Object rootSelection;

        public Scene Scene { get => scene; }

        public bool Enter()
        {
            if (IsActive())
            {
                /// TODO: редактировать
                Debug.LogError("[SceneContext] Enter: является открытым");
                return false;
            }

            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                rootSelection = Selection.activeObject;

                rootScenesSetup = EditorSceneManager.GetSceneManagerSetup();
                scene = NewScene();

                SaveRootSceneViews();
                SceneViewsSetup();

                return true;
            }
            else
            {
                return false;
            }
        }

        public void Exit()
        {
            if (IsActive() == false)
            {
                /// TODO: редактировать
                throw new Exception("[SceneContext] Exit: не является active");
            }

            EditorSceneManager.CloseScene(Scene, removeScene: true);
            EditorSceneManager.RestoreSceneManagerSetup(rootScenesSetup);

            Selection.activeObject = rootSelection;

            LoadRootSceneViews();

            rootScenesSetup = null;
            rootSelection = null;
            sceneViewCaches = null;
        }

        public bool IsActive()
        {
            return SceneManager.GetActiveScene() == Scene;
        }

        private Scene NewScene()
        {
            Scene createdScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
            createdScene.name = Name;

            GameObject behaviourObj = new GameObject(nameof(Behaviour));

            return createdScene;
        }

        #region SceneViews

        private void SaveRootSceneViews()
        {
            ArrayList sceneViews = SceneView.sceneViews;
            sceneViewCaches = new SceneViewCache[sceneViews.Count];

            for (int i = 0; i < sceneViews.Count; i++)
            {
                SceneView sceneView = (SceneView)sceneViews[i];
                sceneViewCaches[i] = new SceneViewCache(sceneView);
            }
        }

        private void LoadRootSceneViews()
        {
            foreach (SceneViewCache cache in sceneViewCaches)
            {
                if (cache.IsInvalid) continue;

                cache.View.sceneViewState.showFog = cache.State.showFog;
                cache.View.sceneViewState.showFlares = cache.State.showFlares;
                cache.View.sceneViewState.alwaysRefresh = cache.State.alwaysRefresh;
                cache.View.sceneViewState.showSkybox = cache.State.showSkybox;
                cache.View.sceneViewState.showImageEffects = cache.State.showImageEffects;
                cache.View.sceneViewState.showParticleSystems = cache.State.showParticleSystems;
            }
        }

        private void SceneViewsSetup()
        {
            foreach (SceneView sceneView in SceneView.sceneViews)
            {
                sceneView.sceneViewState.showFog = false;
                sceneView.sceneViewState.showFlares = false;
                sceneView.sceneViewState.alwaysRefresh = true;
                sceneView.sceneViewState.showSkybox = false;
                sceneView.sceneViewState.showImageEffects = false;
                sceneView.sceneViewState.showParticleSystems = false;
            }
        }

        #endregion

        protected class SceneViewCache
        {

            public SceneView View;
            public SceneView.SceneViewState State;

            public bool IsInvalid => View == null;

            public SceneViewCache(SceneView view)
            {
                View = view;
                State = new SceneView.SceneViewState(view.sceneViewState);
            }
        }

    }

}

#endif
