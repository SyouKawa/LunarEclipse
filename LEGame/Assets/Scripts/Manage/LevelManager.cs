using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using MoreMountains.Tools;


namespace KMGame
{
    [AddComponentMenu("Corgi Engine/Managers/Level Manager")]
    public class LevelManager : MMSingleton<LevelManager>
    {
        public List<Character> Players { get; protected set; }

        [Information("The level bounds are used to constrain the camera's movement, as well as the player character's. You can see it in real time in the scene view as you adjust its size (it's the yellow box).", InformationType.Info, false)]
        /// the level limits, camera and player won't go beyond this point.
        public Bounds LevelBounds = new Bounds(Vector3.zero, Vector3.one * 10);
    }

}