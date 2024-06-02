using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace SilkingSilk
{
    internal class Program
    {
        private static IWindow _window;
        private static GL _gl;

        //Our new abstracted objects, here we specify what the types are.
        private static BufferObject<float> _vbo;
        private static BufferObject<uint> _ebo;
        private static VertexArrayObject<float, uint> _vao;
        private static SilkingSilk.Shader _shader;
        private static float[] _vertices =
        {
            //aPosition     | aTexCoords
            0.5f,  0.5f, 0.0f,  1.0f, 1.0f,
            0.5f, -0.5f, 0.0f,  1.0f, 0.0f,
            -0.5f, -0.5f, 0.0f,  0.0f, 0.0f,
            -0.5f,  0.5f, 0.0f,  0.0f, 1.0f
        };

        private static readonly uint[] _indices =
{
            0, 1, 3,
            1, 2, 3
        };

        private static void Main(string[] args)
        {
            WindowOptions options = WindowOptions.Default with
            {
                Size = new Vector2D<int>(800, 600),
                Title = "My first Silk.NET application!"
            };

            _window = Silk.NET.Windowing.Window.Create(options);

            _window.Load += OnLoad;
            _window.Update += OnUpdate;
            _window.Render += OnRender;
            _window.Closing += OnClose;

            _window.Run();
        }

        private static unsafe void OnLoad()
        {
            IInputContext input = _window.CreateInput();
            for (int i = 0; i < input.Keyboards.Count; i++)
                input.Keyboards[i].KeyDown += KeyDown;

            _gl = GL.GetApi(_window);
            //Instantiating our new abstractions
            _ebo = new BufferObject<uint>(_gl, _indices, BufferTargetARB.ElementArrayBuffer);
            _vbo = new BufferObject<float>(_gl, _vertices, BufferTargetARB.ArrayBuffer);
            _vao = new VertexArrayObject<float, uint>(_gl, _vbo, _ebo);

            //Telling the VAO object how to lay out the attribute pointers
            _vao.VertexAttributePointer(0, 3, VertexAttribPointerType.Float, 7, 0);
            _vao.VertexAttributePointer(1, 4, VertexAttribPointerType.Float, 7, 3);

            _shader = new SilkingSilk.Shader(_gl, "shader.vert", "shader.frag");
        }

        private static void KeyDown(IKeyboard keyboard, Key key, int keyCode)
        {
            if (key == Key.Escape)
                _window.Close();
        }

        private static void OnUpdate(double dt)
        { }

        private static unsafe void OnRender(double dt)
        {
            _gl.Clear((uint)ClearBufferMask.ColorBufferBit);

            //Binding and using our VAO and shader.
            _vao.Bind();
            _shader.Use();
            //Setting a uniform.
            _shader.SetUniform("uBlue", (float)Math.Sin(DateTime.Now.Millisecond / 1000f * Math.PI));

            _gl.DrawElements(PrimitiveType.Triangles, (uint)_indices.Length, DrawElementsType.UnsignedInt, (void*)0);
        }

        private static void OnClose()
        {
            //Remember to dispose all the instances.
            _vbo.Dispose();
            _ebo.Dispose();
            _vao.Dispose();
            _shader.Dispose();
        }
    }
}
