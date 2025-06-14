using System.Collections.Concurrent;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Drawing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.Fonts;

namespace DellArteAPI.Tools;

public interface IImageRenderer
{
    Task<MemoryStream> RenderToStreamAsync(
        string templateSource,
        IEnumerable<CanvasElement> elements,
        IImageFormat format);

    Task<byte[]> RenderToByteArrayAsync(
        string templateSource,
        IEnumerable<CanvasElement> elements,
        IImageFormat format);
}

public abstract class CanvasElement
{
    public AnchorPosition Anchor { get; set; } = AnchorPosition.None;
    public PointF Offset { get; set; }
    public Margins Margins { get; set; } = new Margins();
    public abstract SizeF CalculateSize(IImageProcessingContext context);
    public abstract void Draw(IImageProcessingContext context);
}

public class TextElement : CanvasElement
{
    public string Text { get; set; } = string.Empty;
    public Color Color { get; set; } = Color.Black;
    public string FontFamily { get; set; } = "Alegreya";
    public float FontSize { get; set; } = 14;
    public FontStyle FontStyle { get; set; } = FontStyle.Bold;
    public Font? Font { get; set; }
    public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Left;
    public VerticalAlignment VerticalAlignment { get; set; } = VerticalAlignment.Top;

    private RichTextOptions? _textOptions;

    public override SizeF CalculateSize(IImageProcessingContext context)
    {
        var textBounds = MeasureTextWithAlignment();
        return new SizeF(textBounds.Width, textBounds.Height);
    }
    private FontRectangle MeasureTextWithAlignment()
    {
        return TextMeasurer.MeasureBounds(
            Text,
            new TextOptions(Font!)
            {
                HorizontalAlignment = HorizontalAlignment,
                VerticalAlignment = VerticalAlignment
            });
    }
    public override void Draw(IImageProcessingContext context)
    {
        var position = CalculatePosition(context, CalculateSize(context));
        _textOptions = new RichTextOptions(Font!);
        _textOptions!.Origin = position;
        context.DrawText(_textOptions, Text, Color);
    }
    private PointF CalculatePosition(IImageProcessingContext context, SizeF textSize)
    {
        var imageSize = context.GetCurrentSize();
        return LayoutCalculator.CalculatePosition(
            imageSize,
            textSize,
            Anchor,
            Margins,
            Offset);
    }
}
public class ImageElement : CanvasElement
{
    public string ImageUrl { get; set; }
    public Image<Rgba32> ImageData { get; set; }
    public float Opacity { get; set; } = 1f;
    public float CornerRadius { get; set; } = 1f;

    public override SizeF CalculateSize(IImageProcessingContext context)
    {
        return new SizeF(ImageData.Width, ImageData.Height);
    }

    public override void Draw(IImageProcessingContext context)
    {
        var position = CalculatePosition(context, CalculateSize(context));

        if (CornerRadius > 0)
        {
            var path = ShapeBuilder.BuildRoundedRect(
                position.X,
                position.Y,
                ImageData.Width,
                ImageData.Height,
                CornerRadius);

            context.Clip(path, x => x.Crop(ImageData.Width, ImageData.Height));
            context.DrawImage(ImageData, (Point)position, Opacity);
        }
        else
        {
            context.DrawImage(ImageData, (Point)position, Opacity);
        }
    }

    private PointF CalculatePosition(IImageProcessingContext context, SizeF imageSize)
    {
        var containerSize = context.GetCurrentSize();
        return LayoutCalculator.CalculatePosition(
            containerSize,
            imageSize,
            Anchor,
            Margins,
            Offset);
    }
}
public class BoxElement : CanvasElement
{
    public Color FillColor { get; set; } = Color.Transparent;
    public SizeF Size { get; set; }
    public float CornerRadius { get; set; }

    public override SizeF CalculateSize(IImageProcessingContext context) => Size;

    public override void Draw(IImageProcessingContext context)
    {
        var position = CalculatePosition(context, Size);
        var rect = ShapeBuilder.BuildRoundedRect(
            position.X,
            position.Y,
            Size.Width,
            Size.Height,
            CornerRadius
        );

        context.Fill(FillColor, rect);
    }

    private PointF CalculatePosition(IImageProcessingContext context, SizeF size)
    {
        var containerSize = context.GetCurrentSize();
        return LayoutCalculator.CalculatePosition(
            containerSize,
            size,
            Anchor,
            Margins,
            Offset);
    }
}
public class TextWithBackgroundElement : CanvasElement
{
    private readonly TextElement _textElement;
    private readonly BoxElement _boxElement;
    private float _padding;

    public TextWithBackgroundElement()
    {
        _textElement = new TextElement();
        _boxElement = new BoxElement();
        _padding = 5f;
    }

    public string Text {
        get => _textElement.Text;
        set => _textElement.Text = value; 
    }
    public Color TextColor {
        get => _textElement.Color;
        set => _textElement.Color = value; 
    }
    public string FontFamily
    {
        get => _textElement.FontFamily;
        set => _textElement.FontFamily = value;
    }
    public float FontSize
    {
        get => _textElement.FontSize;
        set => _textElement.FontSize = value;
    }
    public FontStyle FontStyle
    {
        get => _textElement.FontStyle;
        set => _textElement.FontStyle = value;
    }
    public Font? Font {
        get => _textElement.Font;
        set => _textElement.Font = value;
    }
    public Color BackgroundColor {
        get => _boxElement.FillColor;
        set => _boxElement.FillColor = value;
    }
    public float CornerRadius
    {
        get => _boxElement.CornerRadius;
        set => _boxElement.CornerRadius = value;
    }
    public float Padding
    {
        get => _padding;
        set => _padding = value;
    }
    public HorizontalAlignment TextHorizontalAlignment
    {
        get => _textElement.HorizontalAlignment;
        set => _textElement.HorizontalAlignment = value;
    }
    public VerticalAlignment TextVerticalAlignment
    {
        get => _textElement.VerticalAlignment;
        set => _textElement.VerticalAlignment = value;
    }
    public override SizeF CalculateSize(IImageProcessingContext context)
    {
        var textSize = _textElement.CalculateSize(context);

        return new SizeF(
            textSize.Width + _padding * 2,
            textSize.Height + _padding * 2
        );
    }
    public override void Draw(IImageProcessingContext context)
    {
        var fullSize = CalculateSize(context);

        float bgOffsetX = 0;
        float bgOffsetY = 0;

        switch (_textElement.HorizontalAlignment)
        {
            case HorizontalAlignment.Center:
                bgOffsetX = _padding;
                break;
            case HorizontalAlignment.Right:
                bgOffsetX = _padding * 2;
                break;
        }

        switch (_textElement.VerticalAlignment)
        {
            case VerticalAlignment.Center:
                bgOffsetY = _padding * 2;
                break;
            case VerticalAlignment.Bottom:
                bgOffsetY = _padding * 4;
                break;
        }

        _boxElement.Size = fullSize;
        _boxElement.Anchor = Anchor;
        _boxElement.Offset = new PointF(
            Offset.X + bgOffsetX,
            Offset.Y + bgOffsetY
        );
        _boxElement.Margins = Margins;
        _boxElement.Draw(context);

        _textElement.Anchor = Anchor;
        _textElement.Offset = Offset;
        _textElement.Margins = Margins;
        _textElement.Draw(context);
    }
}
public static class LayoutCalculator
{
    public static PointF CalculateAnchorPosition(Size container, AnchorPosition anchor)
    {
        return anchor switch
        {
            AnchorPosition.TopLeft => new PointF(0, 0),
            AnchorPosition.TopCenter => new PointF(container.Width / 2f, 0),
            AnchorPosition.TopRight => new PointF(container.Width, 0),
            AnchorPosition.CenterLeft => new PointF(0, container.Height / 2f),
            AnchorPosition.Center => new PointF(container.Width / 2f, container.Height / 2f),
            AnchorPosition.CenterRight => new PointF(container.Width, container.Height / 2f),
            AnchorPosition.BottomLeft => new PointF(0, container.Height),
            AnchorPosition.BottomCenter => new PointF(container.Width / 2f, container.Height),
            AnchorPosition.BottomRight => new PointF(container.Width, container.Height),
            _ => PointF.Empty
        };
    }

    public static PointF CalculatePosition(
        Size container,
        SizeF elementSize,
        AnchorPosition anchor,
        Margins margins,
        PointF offset)
    {
        var anchorPoint = CalculateAnchorPosition(container, anchor);
        var calculatedX = anchor switch
        {
            AnchorPosition.TopLeft or AnchorPosition.CenterLeft or AnchorPosition.BottomLeft =>
                anchorPoint.X + margins.Left + offset.X,

            AnchorPosition.TopRight or AnchorPosition.CenterRight or AnchorPosition.BottomRight =>
                anchorPoint.X - elementSize.Width - margins.Right + offset.X,

            _ => anchorPoint.X - elementSize.Width / 2 + offset.X
        };

        var calculatedY = anchor switch
        {
            AnchorPosition.TopLeft or AnchorPosition.TopCenter or AnchorPosition.TopRight =>
                anchorPoint.Y + margins.Top + offset.Y,

            AnchorPosition.BottomLeft or AnchorPosition.BottomCenter or AnchorPosition.BottomRight =>
                anchorPoint.Y - elementSize.Height - margins.Bottom + offset.Y,

            _ => anchorPoint.Y - elementSize.Height / 2 + offset.Y
        };

        return new PointF(calculatedX, calculatedY);
    }

    public static PointF CalculatePositionWithMargins(
        PointF anchorPoint,
        SizeF elementSize,
        Margins margins,
        PointF offset)
    {
        return new PointF(
            anchorPoint.X + margins.Left - margins.Right + offset.X,
            anchorPoint.Y + margins.Top - margins.Bottom + offset.Y
        );
    }
}
public static class ShapeBuilder
{
    public static IPath BuildRoundedRect(float x, float y, float width, float height, float radius)
    {
        var builder = new PathBuilder();
        radius = Math.Clamp(radius, 0, Math.Min(width, height) / 2);

        builder.AddLine(x + width - 2 * radius, y - radius * 2, x + width - radius, y);
        builder.AddArc(
            x + width - 2 * radius,
            y,
            2 * radius,
            2 * radius,
            0,
            270,
            90
        );

        builder.AddLine(x + width, y + height - 2 * radius, x + width, y + height - radius);
        builder.AddArc(
            x + width - 2 * radius,
            y + height - 2 * radius,
            2 * radius,
            2 * radius,
            0,
            0,
            90
        );

        builder.AddLine(x + radius * 2, y + height, x + radius, y + height);
        builder.AddArc(
            x,
            y + height - 2 * radius,
            2 * radius,
            2 * radius,
            0,
            90,
            90
        );

        builder.AddLine(x - radius * 2, y - radius, x - radius * 2, y + radius);
        builder.AddArc(
            x,
            y,
            2 * radius,
            2 * radius,
            0,
            180,
            90
        );

        return builder.Build();
    }
}
public record Margins(
    float Left = 0,
    float Top = 0,
    float Right = 0,
    float Bottom = 0
);
public class ImageRenderer : IDisposable, IImageRenderer
{
    private readonly HttpClient _httpClient;
    private readonly ConcurrentDictionary<string, Image<Rgba32>> _imageCache;
    internal readonly FontCollection _fonts;

    public ImageRenderer(IHttpClientFactory client)
    {
        _httpClient = client.CreateClient();
        _imageCache = new ConcurrentDictionary<string, Image<Rgba32>>();
        _fonts = new FontCollection();
        _fonts.Add(System.IO.Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "Fonts", "Alegreya.ttf"));
        _fonts.AddSystemFonts();
    }

    private async Task<Image> RenderTemplateAsync(
        string templateSource,
        IEnumerable<CanvasElement> elements)
    {
        var template = await GetTemplateAsync(templateSource);
        await PrepareElements(elements);
        return await RenderInternalAsync(template.Clone(), elements);
    }
    private async Task PrepareElements(IEnumerable<CanvasElement> elements)
    {
        foreach (var element in elements)
        {
            if (element is TextElement tElement)
            {
                tElement.Font = new Font(
                    _fonts.Get(tElement.FontFamily),
                    tElement.FontSize,
                    tElement.FontStyle);
            }
            else if (element is TextWithBackgroundElement twbElement)
            {
                twbElement.Font = new Font(
                    _fonts.Get(twbElement.FontFamily),
                    twbElement.FontSize,
                    twbElement.FontStyle);
            }
            else if (element is ImageElement iElement)
            {
                if (_imageCache.TryGetValue(iElement.ImageUrl, out var cachedImage))
                {
                    iElement.ImageData = cachedImage;
                    continue;
                }
                
                iElement.ImageData = await LoadImageAsync(iElement.ImageUrl);
                _imageCache.TryAdd(iElement.ImageUrl, iElement.ImageData);
            }
        }
    }

    private async Task<Image<Rgba32>> GetTemplateAsync(string templateSource)
    {
        if (_imageCache.TryGetValue(templateSource, out var cachedImage))
            return cachedImage;

        var image = await LoadImageAsync(templateSource);
        _imageCache.TryAdd(templateSource, image);
        return image;
    }

    private async Task<Image<Rgba32>> LoadImageAsync(string source)
    {
        if (Uri.IsWellFormedUriString(source, UriKind.Absolute))
        {
            var response = await _httpClient.GetAsync(source);
            response.EnsureSuccessStatusCode();
            var stream = await response.Content.ReadAsStreamAsync();
            return await Image.LoadAsync<Rgba32>(stream);
        }

        return await Image.LoadAsync<Rgba32>(source);
    }

    private static async Task<Image> RenderInternalAsync(Image template, IEnumerable<CanvasElement> elements)
    {
        try
        {
            template.Mutate(ctx =>
            {
                foreach (var element in elements.OrderBy(e => e is BoxElement ? 0 : 1))
                {
                    element.Draw(ctx);
                }
            });

            return template;
        }
        finally
        {
            await Task.Yield();
        }
    }

    public async Task<MemoryStream> RenderToStreamAsync(
        string templateSource,
        IEnumerable<CanvasElement> elements,
        IImageFormat format)
    {
        using var image = await RenderTemplateAsync(templateSource, elements);
        var memoryStream = new MemoryStream();
        await image.SaveAsync(memoryStream, format);
        memoryStream.Position = 0;
        return memoryStream;
    }

    public async Task<byte[]> RenderToByteArrayAsync(
        string templateSource,
        IEnumerable<CanvasElement> elements,
        IImageFormat format)
    {
        using var stream = await RenderToStreamAsync(templateSource, elements, format);
        return stream.ToArray();
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        foreach (var image in _imageCache.Values)
        {
            image.Dispose();
        }
        _imageCache.Clear();
    }
}

public enum AnchorPosition
{
    TopLeft,
    TopCenter,
    TopRight,
    CenterLeft,
    Center,
    CenterRight,
    BottomLeft,
    BottomCenter,
    BottomRight,
    None
}