# Generación Procedural de Contenido 💻⚡

## Contenido:
- [Introducción](#introducción-)
- [Algoritmo "Diamante Cuadrado"](#algoritmo-diamante-cuadrado)
- [Demostración y proyecto en conjunto](#demostración-y-proyecto-en-conjunto)
- [Próximas mejoras](#próximas-mejoras-)
- [Recursos adicionales](#recursos-adicionales)

> Este algoritmo y el proyecto en general no hubiera sido posible sin la guía y apoyo constante de nuestros asesores, la Dra. Rocío Lizárraga y el Dr. Uriel Haile,
> además de todo el increíble equipo de trabajo; Martín, Luis, Bertani, Michel y Alejandro 💛

### Introducción 📖
A pesar de que no exista una definición formal, la **generación procedural de contenido** *se entiende como una forma de 
creación automática de contenido mediante el uso de algoritmos (o procedimientos) en lugar de crearlo de forma manual.*
Este contenido (texturas, terrenos, narrativas, plantas, ecosistemas, ríos, redes de caminos, mecánicas, dinámicas, etc.)
es generado con *entrada de información nula, limitada o indirecta de los programadores.* Sus principales fortalezas recaen
en sus capacidades para comprimir información (Sánchez et al. 2013).

Es un ámbito tecnológico cada vez más importante en el diseño moderno de la interacción persona-computadora. 
**La personalización de la experiencia del usuario mediante el modelado afectivo y cognitivo**, junto con el ajuste en tiempo
real de los contenidos en función de las necesidades y preferencias del usuario son pasos importantes hacia una generación
procedural de contenido (PCG) eficaz y significativa (G. N. Yannakakis and J. Togelius, 2011).

El videojuego es uno de los medios más representativos cuando se trata de aplicaciones de la creación de contenido. 
Así mismo, uno de sus puntos más fuertes es la **síntesis de emociones complejas a través de patrones afectivos y cognitivos
que se generan durante el gameplay del mismo.** Para lograr este objetivo, hay muchos elementos que deben coexistir en una
sinergia muy precisa. Dentro de esos elementos los terrenos y todo el ambiente en el que se desarrolla la historia o idea
de un videojuego juegan un papel crucial.

[**¡ Revisa el artículo completo aquí :octocat: ¡**](https://www.jovenesenlaciencia.ugto.mx/index.php/jovenesenlaciencia/article/view/3984/3470)

Durante la edición XXVIII del Verano de la Ciencia UG, estuvimos explorando diversas aplicaciones de la generación procedural de contenido,
en este repositorio se encuentra una **implementación del algoritmo "Diamond Square"**; un algoritmo básico para introducirse
en estas áreas, utilizado para la generación de mapas
de altura en 2 dimensiones.

### Algoritmo "Diamante Cuadrado"
Partimos de una **matriz de dos dimensiones con una longitud de n x n** (*la superficie debe ser cuadrada para garantizar 
mejores resultados*). Exceptuando los valores de las 4 esquinas que delimitan el terreno y se generan de manera aleatoria,
el resto de los valores de la matriz se calculan a partir del algoritmo, en los siguientes dos pasos:

**1. Paso del diamante :diamonds:**

El paso del diamante: Para cada cuadrado de la matriz, establece que el punto medio de ese cuadrado sea la media de los 
cuatro puntos de las esquinas más un valor aleatorio.

**2. Paso del cuadrado**

Para cada diamante de la matriz, establece que el punto medio de ese diamante sea la media de los cuatro puntos de las 
esquinas más un valor aleatorio.

En cada iteración, el rango aleatorio que se suma al promedio se divide a la mitad para garantizar una distribución 
donde a partir de una altura máxima en un punto del terreno, sus puntos vecinos vayan disminuyendo su altura poco a poco.

En algunos casos, el paso del Cuadrado donde los puntos situados en los bordes de la matriz tendrán sólo tres valores 
adyacentes establecidos en algunos casos, hay varias maneras de solucionar este problema, pero en este proyecto lo 
solucionamos sacando la media de los valores que se encuentran adyacentes.

![Procedure](https://janert.me/blog/2022/the-diamond-square-algorithm-for-terrain-generation/grid.png)

> Sí, parece mucho más complejo cuando ves los cubitos apilados y de colores 😉
### Demostración y proyecto en conjunto

Te invito a que revises el video completo que habla más a fondo de todos los proyectos desarrollados en este verano de
investigación :raised_hands:

- [:tv: **YouTube**](https://www.youtube.com/watch?v=z2KINXBTWxc)



[![Official Video](./repoAssets/demoDiamondSquare.gif)](https://www.youtube.com/watch?v=z2KINXBTWxc "Demo")

### Próximas mejoras ✅
- [ ] Generar una implementación en Unreal Engine y con un componente de mapas de altura
- [ ] Optimizar la generación de elementos para mejorar el rendimiento del programa
- [ ] Parametrizar y experimentar con más variaciones entre los valores pseudo-aleatorios
- [ ] Nuevos algoritmos de generación procedural

### Recursos adicionales
- [White Box Dev](https://youtu.be/4GuAV1PnurU)
- [The Game of life in 3D](https://www.youtube.com/watch?v=6O9CLARsJ04)
