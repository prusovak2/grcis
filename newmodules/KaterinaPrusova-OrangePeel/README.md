# Extension: Orange Peel Bump Texture

### Author: Kateřina Průšová

### Category: Texture

### Namespace: Rendering.KaterinaPrusova

### Class name:  OrangePeelBumpTexture : ITexture

### ITimeDependent: No

### Source file: OrangePeelBumpTexture.cs

A simple 3D texture simulating the structure of an orange peel. Modifies the surface normal vectors to make the surface appear bumpy. Modifies the color and the PhongMaterial of the object to simulate the color and the gloss of the orange peel. Designed to be used with spheres. Won't work properly with other objects. 

**Texture constructor signature:**

```csharp
public OrangePeelBumpTexture (double freq = 0.4, double amplitude = 0.6, double struc = 1, double color = 0.2, int seed = 73)
```

`seed` : seed for the underlying pseudorandom number generator, can be used to deterministically create a slightly varying oranges

`freq`: influences the frequency of the bumps on the surface of the orange

`amplitude`: influences the amplitude of the bumps on the surface of the orange

`struct`: influences the overall appearance of the bumps on the surface of the orange

`color`: influences the shade of the orange, texture color is orange with randomly distributed red and light orange dots, `color` parameter specifies, which color of dots should be more dominating (closer to 0 ... red, closer to 1... light orange). 

**Parameters `freq`, `amplitude`, `struc` and `color` should belong to [0, 1] interval. Values higher than 1 are going to be replaced by 1, values lower that 0 are going to be replace by 0.**

An arbitrary integer can be passed  as the `seed`.

**Improved Perlin Noise** https://developer.download.nvidia.com/books/HTML/gpugems/gpugems_ch05.html is used to make the texture seem more arbitrary and natural. The noise implementation was not the part of the assignment, so it was taken from here  https://gist.github.com/Flafla2/1a0b9ebef678bbce3215 and slightly adjusted.

Normal vector are perturbed by subtracting the gradient of the noise function. This approach was suggested by Ken Perlin (https://developer.download.nvidia.com/books/HTML/gpugems/gpugems_ch05.html). It works fine for the 3D texture, even though Blinn claims it to be incorrect (http://elibrary.lt/resursai/Leidiniai/Litfund/Lithfund_leidiniai/IT/Texturing.and.Modeling.-.A.Procedural.Approach.3rd.edition.eBook-LRN.pdf, page 171) and suggests a different approach (https://www.semanticscholar.org/paper/Simulation-of-wrinkled-surfaces-Blinn/83fb6d2721cd26be4964882ce929ff8c98ebb688) that I did not found to be useful while implementing the 3D texture as all my attempts to implement it created the singularities on the poles of the sphere.

