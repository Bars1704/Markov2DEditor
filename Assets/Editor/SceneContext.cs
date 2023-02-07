#if UNITY_EDITOR

using System;
using System.Collections;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneContext
{
    private const string Name = "Simulation";

    public Scene Scene { get; private set; }
    private SceneSetup[] rootScenesSetup;
    private SceneViewCache[] sceneViewCaches;
    public GameObject rootGameObject { get; private set; }

    public bool Enter()
    {
        if (IsActive())
        {
            Debug.LogError("[SceneContext] Enter: Already opened");
            return false;
        }

        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            rootScenesSetup = EditorSceneManager.GetSceneManagerSetup();
            Scene = NewScene();

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
            throw new Exception("[SceneContext] Exit: Already exit");
        }

        EditorSceneManager.CloseScene(Scene, removeScene: true);
        EditorSceneManager.RestoreSceneManagerSetup(rootScenesSetup);
        
        LoadRootSceneViews();

        rootScenesSetup = null;
        rootGameObject = null;
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

        GameObject behaviourObj = new GameObject("SimulationRoot");
        rootGameObject = behaviourObj;
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


#endif