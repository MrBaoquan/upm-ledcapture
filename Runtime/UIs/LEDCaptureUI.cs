using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UNIHper;
using UNIHper.UI;
using DNHper;
using UniRx.Triggers;
using System.Linq;
using TMPro;
using HSVPicker;
using System;

[UIPage(Asset = "LEDCaptureUI", Order = -1, Type = UIType.Normal)]
public class LEDCaptureUI : UIBase
{
    RectTransform ledRoot => this.Get<RectTransform>("led_root");

    public DynamicTexture GetLEDTexture(string ID)
    {
        return ledRoot.Get<DynamicTexture>(ID);
    }

    public void AddLEDTexture(string ID, Rect rect, Texture2D texture = null)
    {
        var _newLED = new GameObject(ID);
        var _ledTrans = _newLED.AddComponent<RectTransform>();
        _ledTrans.SetParent(ledRoot);

        _ledTrans.AddComponent<RawImage>();
        _ledTrans
            .AddComponent<DynamicTexture>()
            .InitializeTexture((int)rect.width, (int)rect.height);

        _ledTrans.pivot = new Vector2(0, 1);
        _ledTrans.anchorMin = new Vector2(0, 1);
        _ledTrans.anchorMax = new Vector2(0, 1);
        _ledTrans.sizeDelta = new Vector2(rect.width, rect.height);
        _ledTrans.anchoredPosition = new Vector2(rect.x, -rect.y);
    }

    private int LEDIndex => this.Get<TMP_Dropdown>("led_List").value;

    // Called when this ui is loaded
    protected override void OnLoaded()
    {
        var _optPanel = this.Get<RectTransform>("option_panel");
        _optPanel.EnableDragMove();

        Managements.Config
            .Get<LEDConfig>()
            .LEDScreens.ForEach(_ =>
            {
                AddLEDTexture(_.ID, new Rect(_.position.x, _.position.y, _.size.x, _.size.y));
            });

        this.Get<TMP_Dropdown>("led_List").options = Managements.Config
            .Get<LEDConfig>()
            .LEDScreens.Select(_ => new TMP_Dropdown.OptionData(_.ID))
            .ToList();

        var _colorImage = this.Get("input_color").Get<Image>("input_value");
        _colorImage
            .OnPointerClickAsObservable()
            .Subscribe(_ =>
            {
                this.Get("Picker 2.0").SetActive(true);
            });

        this.Get<Image>("btn_confirm")
            .OnPointerClickAsObservable()
            .Subscribe(_ =>
            {
                _colorImage.color = this.Get<ColorPicker>("Picker 2.0").CurrentColor;
                this.Get("Picker 2.0").SetActive(false);
            });

        ReactiveProperty<string> xPos = new ReactiveProperty<string>();
        ReactiveProperty<string> yPos = new ReactiveProperty<string>();

        this.Get("input_x")
            .Get<InputField>("input_value")
            .OnValueChangedAsObservable()
            .Subscribe(_ =>
            {
                xPos.Value = _;
            });

        this.Get("input_y")
            .Get<InputField>("input_value")
            .OnValueChangedAsObservable()
            .Subscribe(_ =>
            {
                yPos.Value = _;
            });

        this.Get<Button>("btn_apply")
            .OnClickAsObservable()
            .Subscribe(_ =>
            {
                ledRoot
                    .GetChild(LEDIndex)
                    .GetComponent<DynamicTexture>()
                    .SetPixel(xPos.Value.Parse2Int(), yPos.Value.Parse2Int(), _colorImage.color);
            });

        registerMoveEvents();

        Observable
            .EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.F5))
            .Subscribe(_ =>
            {
                _optPanel.SetActive(!_optPanel.gameObject.activeSelf);
            })
            .AddTo(this);

        _optPanel.SetActive(false);
        this.Show();
    }

    private void registerMoveEvents()
    {
        Observable
            .EveryUpdate()
            .Where(_ => this.isShowing)
            .Subscribe(_ =>
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    var _rectTrans = ledRoot.GetChild(LEDIndex).GetComponent<RectTransform>();
                    _rectTrans.anchoredPosition += new Vector2(-1f, 0.0f);
                    Debug.Log(_rectTrans.anchoredPosition);
                }
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    var _rectTrans = ledRoot.GetChild(LEDIndex).GetComponent<RectTransform>();
                    _rectTrans.anchoredPosition += new Vector2(1f, 0.0f);
                    Debug.Log(_rectTrans.anchoredPosition);
                }

                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    var _rectTrans = ledRoot.GetChild(LEDIndex).GetComponent<RectTransform>();
                    _rectTrans.anchoredPosition += new Vector2(0.0f, 1f);
                    Debug.Log(_rectTrans.anchoredPosition);
                }

                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    var _rectTrans = ledRoot.GetChild(LEDIndex).GetComponent<RectTransform>();
                    _rectTrans.anchoredPosition += new Vector2(0.0f, -1f);
                    Debug.Log(_rectTrans.anchoredPosition);
                }

                if (Input.anyKeyDown)
                {
                    var _pos = ledRoot
                        .GetChild(LEDIndex)
                        .GetComponent<RectTransform>()
                        .anchoredPosition;
                    _pos.y *= -1;
                    Managements.Config.Get<LEDConfig>().LEDScreens[LEDIndex].position =
                        new SerializableVector2(_pos.x, _pos.y);
                    Managements.Config.Save<LEDConfig>();
                }
            })
            .AddTo(this);
    }

    // Called when this ui is shown
    protected override void OnShown() { }

    // Called when this ui is hidden
    protected override void OnHidden() { }
}
