using System;
using System.Drawing;
using System.Threading;
using Clipping2D.Drawer;
using Open3D.Geometry;
using Open3D.Geometry.Factory;
using Open3D.Geometry.Polyhedron;
using Open3D.Math;
using Open3D.Rendering;
using SdlApplication.Drawer;
using SDL2;

namespace SdlApplication.Window
{
    public class SdlWindow
    {
        private readonly int _renderLoopTimeoutMs = 10;
        private readonly double _rotationAngle = Math.PI / 90;
        private readonly int _observerMoveStep = 10;
        private readonly int _displayMoveStep = 10;

        private readonly int _screenWidth;
        private readonly int _screenHeight;
        private readonly string _title;

        private IntPtr _renderer;
        private IntPtr _window;

        private IScene _scene;
        private readonly IPolygonDrawer _visibleFacetDrawer;
        private readonly IPolygonDrawer _notVisibleFacetDrawer;

        public SdlWindow(string title, int screenWidth, int screenHeight)
        {
            _title = title;
            _screenHeight = screenHeight;
            _screenWidth = screenWidth;
            _visibleFacetDrawer = new VisibleFacetDrawer();
            _notVisibleFacetDrawer = new NotVisibleFacetDrawer();
        }

        private void InitializeScene()
        {
            IPolyhedron3D polyhedron = PolyhedronBuilder.CreateSimplePolyhedron(200, 200, 200,
                new HomogeneousPoint3D(0, 0, 0, 1), new HomogeneousPoint3D(0, 0, 0, 1));
            _scene = new SingleObjectScene(new HomogeneousPoint3D(0, 0, -500, 1), polyhedron, 450);
            _scene.Initialize();
        }

        public void Open()
        {
            var thred = new Thread(() =>
            {
                SDL.SDL_Init(SDL.SDL_INIT_EVERYTHING);
                _window = SDL.SDL_CreateWindow(_title, SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED,
                    _screenWidth, _screenHeight, SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
                _renderer = SDL.SDL_CreateRenderer(_window, -1, SDL.SDL_RendererFlags.SDL_RENDERER_ACCELERATED);

                InitializeScene();
                WindowProcedure();

                SDL.SDL_DestroyRenderer(_renderer);
                SDL.SDL_DestroyWindow(_window);
                SDL.SDL_Quit();
            });
            thred.Start();
            thred.Join();
        }

        private void WindowProcedure()
        {
            bool exit = false;
            while (!exit)
            {
                SDL.SDL_Event sdlEvent;
                SDL.SDL_PollEvent(out sdlEvent);
                switch (sdlEvent.type)
                {
                    case SDL.SDL_EventType.SDL_QUIT:
                    {
                        exit = true;
                        break;
                    }
                    case SDL.SDL_EventType.SDL_KEYDOWN:
                    {
                        var key = sdlEvent.key;
                        switch (key.keysym.sym)
                        {
                            case SDL.SDL_Keycode.SDLK_w:
                                _scene.RotateAroundAxis(Axis3D.OX, -_rotationAngle);
                                break;
                            case SDL.SDL_Keycode.SDLK_s:
                                _scene.RotateAroundAxis(Axis3D.OX, _rotationAngle);
                                break;
                            case SDL.SDL_Keycode.SDLK_a:
                                _scene.RotateAroundAxis(Axis3D.OY, _rotationAngle);
                                break;
                            case SDL.SDL_Keycode.SDLK_d:
                                _scene.RotateAroundAxis(Axis3D.OY, -_rotationAngle);
                                break;
                            case SDL.SDL_Keycode.SDLK_q:
                                _scene.RotateAroundAxis(Axis3D.OZ, -_rotationAngle);
                                break;
                            case SDL.SDL_Keycode.SDLK_e:
                                _scene.RotateAroundAxis(Axis3D.OZ, _rotationAngle);
                                break;
                            case SDL.SDL_Keycode.SDLK_z:
                                _scene.RotateAroundVector(-_rotationAngle);
                                break;
                            case SDL.SDL_Keycode.SDLK_x:
                                _scene.RotateAroundVector(_rotationAngle);
                                break;
                            case SDL.SDL_Keycode.SDLK_r:
                                _scene.MoveObserverTo(new HomogeneousPoint3D(0, 0, _observerMoveStep, 1));
                                break;
                            case SDL.SDL_Keycode.SDLK_f:
                                _scene.MoveObserverTo(new HomogeneousPoint3D(0, 0, -_observerMoveStep, 1));
                                break;
                            case SDL.SDL_Keycode.SDLK_KP_8:
                                _scene.MoveObserverTo(new HomogeneousPoint3D(0, _observerMoveStep, 0, 1));
                                break;
                            case SDL.SDL_Keycode.SDLK_KP_2:
                                _scene.MoveObserverTo(new HomogeneousPoint3D(0, -_observerMoveStep, 0, 1));
                                break;
                            case SDL.SDL_Keycode.SDLK_KP_6:
                                _scene.MoveObserverTo(new HomogeneousPoint3D(-_observerMoveStep, 0, 0, 1));
                                break;
                            case SDL.SDL_Keycode.SDLK_KP_4:
                                _scene.MoveObserverTo(new HomogeneousPoint3D(_observerMoveStep, 0, 0, 1));
                                break;
                                case SDL.SDL_Keycode.SDLK_t:
                                _scene.MoveDisplay(_displayMoveStep);
                                break;
                            case SDL.SDL_Keycode.SDLK_g:
                                _scene.MoveDisplay(-_displayMoveStep);
                                break;
                            }
                        break;
                    }
                }
                RenderScene();
                Thread.Sleep(_renderLoopTimeoutMs);
            }
        }

        // Формат цвета в HEX коде:
        //     0xRRGGBB00
        //  где R: от 00 до FF
        //      G: от 00 до FF
        //      B: от 00 до FF 
        private void RenderScene()
        {
            SDL.SDL_SetRenderDrawColor(_renderer, 0, 0, 0, 255);
            SDL.SDL_RenderClear(_renderer);
            SDL.SDL_SetRenderDrawColor(_renderer, 255, 255, 255, 255);

            int width, height;
            SDL.SDL_GetWindowSize(_window, out width, out height);

            var screenCenter = new Point
            {
                X = -width / 2,
                Y = -height / 2
            };

            _scene.Render(_renderer, _visibleFacetDrawer, _notVisibleFacetDrawer, screenCenter);

            SDL.SDL_RenderPresent(_renderer);
        }
    }
}
