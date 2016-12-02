#version 120

uniform sampler2D texFramebuffer;
uniform float mipmapLevel;

varying vec4 Colour;
varying vec2 Texcoord;

const float blurSize = 1 / 200.0f;

void main()
{
    vec4 colourSum = vec4(0.0f);
    
    for (int x = -4; x <= 4; x++)
    {
        for (int y = -4; y <= 4; y++)
        {
            float dx = Texcoord.x + x * blurSize;
            float dy = Texcoord.y + y * blurSize;
            vec4 pixelColour = texture2D(texFramebuffer, vec2(dx, dy), mipmapLevel);
            colourSum += pixelColour * pixelColour;
        }
    }
    
    colourSum /= 81;
    colourSum = sqrt(colourSum);
    
    gl_FragColor = Colour * colourSum;
}