A whiteboard editor for Approximately Up

[Download](https://raw.githubusercontent.com/quakec/AU_Whiteboard_Editor/9760639e52cc81c5d91e36966af36724ac1be817/Binaries/2.26.223.2053/AU_Whiteboard_Editor.exe)

What is this sorcery you might ask?! Well this tool allows you to open any image, format it and overwrite your whiteboards, it includes automatic ship enumeration, dithering and crop tools with the ability to filter the palette colours.

How to use: Either order, pick your ships's whiteboard from the treeview and click Open Image. At this point you'll be able to click the overwrite button but before you do that, there are various adjustment tools at your disposal, play around to see what looks best.

Some of images and colour space combinations produce heavily weighted palette colours, this is why I've included the option to filter them. In fact removing green has helped in a lot of cases with blues and even greens.

Dithering doesn't work too well with noisy images, but remember we're working with just 8 colours. There is no single setting that will work for all images, the best thing to do is try each colour space with each of the dithering techniques and play with the brightness to achieve the best results.

The slider options provide a small adjustment to the presence of a colour, it can help but doesn't for some dither/colour space combinations because the algorithms are complex.

Actual whiteboard image resolutions are 384 x 256, that's a 3:2 ratio for processing images outside of the application.

Disclaimer: I have provided a compiled binary (in the Binaries folder) for your convenience but I do not expect you to trust it. You may download the source code and build the project in Visual Studio instead.

Yes this could've been hosted on here as a web app, but I wanted to make it easy to select your ship and its whiteboards instead of having to rummage around in your blueprints folder.

Thanks to RJ#0514 for reminding me that this needed updating to support colour.

<img width="1070" height="889" alt="image" src="https://github.com/user-attachments/assets/8ab5eb88-0936-43be-a31e-2f0adfe65474" />
<img width="1070" height="889" alt="image" src="https://github.com/user-attachments/assets/ee38a7bc-79bf-4f5f-96d2-fe1e3a3fd02e" />
<img width="1019" height="818" alt="Screenshot2" src="https://github.com/user-attachments/assets/a2757a91-4a28-4ace-912c-77841de90b36" />
