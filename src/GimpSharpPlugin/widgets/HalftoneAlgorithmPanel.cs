using System;
using System.Collections.Generic;
using System.Linq;
using Gtk;
using Halftone;

namespace Gimp.HalftoneLab
{
    public class HalftoneAlgorithmPanel : Notebook
    {
        private HalftoneAlgorithm module;
        public HalftoneAlgorithm Module {
            get { return module; }
            set {
                module = (value != null) ? value : new HalftoneAlgorithm();
                preResizePanel.Module = module.PreResize;
                preDotGainEnabledCheckButton.Active = module.PreDotGain != null;
                preDotGainGammaSpinButton.Sensitive = module.PreDotGain != null;
                preSharpenEnabledCheckButton.Active = module.PreSharpen != null;
                preSharpenAmountHScale.Sensitive = module.PreSharpen != null;
                halftoneMethodSelector.assignModule(module.Method, false);
                postResizePanel.Module = module.PostResize;
                supersamplingCheckButton.Active = module.SupersamplingEnabled;
                postSmoothenEnabledCheckButton.Active = module.PostSmoothen != null;
                postSmoothenRadiusSpinButton.Sensitive = module.PostSmoothen != null;
                if (ModuleChanged != null) {
                    ModuleChanged(this, new EventArgs());
                }
            }
        }

        public event EventHandler ModuleChanged;

        private ResizePanel preResizePanel;
        private CheckButton preDotGainEnabledCheckButton;
        private SpinButton preDotGainGammaSpinButton;
        private CheckButton preSharpenEnabledCheckButton;
        private HScale preSharpenAmountHScale;
        private SubmoduleSelector<HalftoneMethod> halftoneMethodSelector;
        private ResizePanel postResizePanel;
        private CheckButton supersamplingCheckButton;
        private CheckButton postSmoothenEnabledCheckButton;
        private SpinButton postSmoothenRadiusSpinButton;

        public HalftoneAlgorithmPanel()
            : this(new HalftoneAlgorithm()) { }

        public HalftoneAlgorithmPanel(HalftoneAlgorithm existingModule)
        {
            BorderWidth = 3;

            VBox preProcessingVBox = new VBox();
            Frame preResizeFrame = new Frame("Resize") { BorderWidth = 3 };
            Frame preDotGainFrame = new Frame("Dot gain correction") { BorderWidth = 3 };
            Frame preSharpenFrame = new Frame("Sharpen") { BorderWidth = 3 };
            Frame halftoneMethodFrame = new Frame("Halftone method") { BorderWidth = 3 };
            VBox postProcessingVBox = new VBox();
            Frame postResizeFrame = new Frame("Resize") { BorderWidth = 3 };
            Frame postSmoothenFrame = new Frame("Smoothen") { BorderWidth = 3 };

            // -------- pre-processing --------

            // ---- resize ----

            preResizePanel = new ResizePanel();
            preResizePanel.ModuleChanged += delegate
            {
                Module.PreResize = preResizePanel.Module;
                if (ModuleChanged != null) {
                    ModuleChanged(this, new EventArgs());
                }
            };
            preResizeFrame.Add(preResizePanel);
            preProcessingVBox.PackStart(preResizeFrame);

            // ---- dot gain ----

            preDotGainEnabledCheckButton = new CheckButton("Enabled");
            preDotGainEnabledCheckButton.Toggled += delegate
            {
                bool enabled = preDotGainEnabledCheckButton.Active;
                if (!enabled) {
                    Module.PreDotGain = null;
                } else if (Module.PreDotGain == null) {
                    Module.PreDotGain =
                        new HalftoneAlgorithm.GammaCorrection();
                }
                if (Module.PreDotGain != null) {
                    preDotGainGammaSpinButton.Value =
                        ((HalftoneAlgorithm.GammaCorrection)
                        Module.PreDotGain).Gamma;
                }
                preDotGainGammaSpinButton.Sensitive = enabled;
                if (ModuleChanged != null) {
                    ModuleChanged(this, new EventArgs());
                }
            };
            preDotGainGammaSpinButton = new SpinButton(0.1, 10, 0.1);
            preDotGainGammaSpinButton.Changed += delegate
            {
                ((HalftoneAlgorithm.GammaCorrection)Module.PreDotGain).Gamma =
                    preDotGainGammaSpinButton.Value;
                if (ModuleChanged != null) {
                    ModuleChanged(this, new EventArgs());
                }
            };
            Table preDotGainTable = new Table(2, 2, false) { ColumnSpacing = 3, RowSpacing = 3, BorderWidth = 5 };
            preDotGainTable.Attach(preDotGainEnabledCheckButton, 0, 2, 0, 1,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);

            preDotGainTable.Attach(new Label("Gamma:") { Xalign = 0.0f },
                0, 1, 1, 2, AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
            preDotGainTable.Attach(preDotGainGammaSpinButton, 1, 2, 1, 2,
                AttachOptions.Fill, AttachOptions.Shrink, 0, 0);

            preDotGainFrame.Add(preDotGainTable);
            preProcessingVBox.PackStart(preDotGainFrame);

            // ---- sharpen ----

            preSharpenEnabledCheckButton = new CheckButton("Enabled");
            preSharpenEnabledCheckButton.Toggled += delegate
            {
                bool enabled = preSharpenEnabledCheckButton.Active;
                if (!enabled) {
                    Module.PreSharpen = null;
                } else if (Module.PreSharpen == null) {
                    Module.PreSharpen =
                        new HalftoneAlgorithm.Sharpen();
                }
                if (Module.PreSharpen != null) {
                    preSharpenAmountHScale.Value = Module.PreSharpen.Amount;
                }
                preSharpenAmountHScale.Sensitive = enabled;
                if (ModuleChanged != null) {
                    ModuleChanged(this, new EventArgs());
                }
            };
            preSharpenAmountHScale = new HScale(0, 1, 0.01);
            preSharpenAmountHScale.ChangeValue += delegate
            {
                Module.PreSharpen.Amount = preSharpenAmountHScale.Value;
                if (ModuleChanged != null) {
                    ModuleChanged(this, new EventArgs());
                }
            };
            Table preSharpenTable = new Table(2, 2, false)
                { ColumnSpacing = 3, RowSpacing = 3, BorderWidth = 5 };
            preSharpenTable.Attach(preSharpenEnabledCheckButton, 0, 2, 0, 1,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);

            preSharpenTable.Attach(new Label("Amount:") { Xalign = 0.0f },
                0, 1, 1, 2, AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
            preSharpenTable.Attach(preSharpenAmountHScale, 1, 2, 1, 2,
                AttachOptions.Fill, AttachOptions.Shrink, 0, 0);

            preSharpenFrame.Add(preSharpenTable);
            preProcessingVBox.PackStart(preSharpenFrame);
            
            AppendPage(preProcessingVBox, new Label("Pre-processing"));

            // -------- halftone method --------

            halftoneMethodSelector =
                new SubmoduleSelector<HalftoneMethod>(HalftoneMethod.createDefault())
                { BorderWidth = 5 };
            halftoneMethodSelector.ModuleChanged += delegate
            {
                if (Module != null) {
                    Module.Method = halftoneMethodSelector.Module;
                    if (ModuleChanged != null) {
                        ModuleChanged(this, new EventArgs());
                    }
                }
            };
            halftoneMethodFrame.Add(halftoneMethodSelector);
            AppendPage(halftoneMethodFrame, new Label("Halftone method"));


            // -------- post-processing --------

            // ---- resize ----

            postResizePanel = new ResizePanel();
            postResizePanel.ModuleChanged += delegate
            {
                Module.PostResize = postResizePanel.Module;
                if (ModuleChanged != null) {
                    ModuleChanged(this, new EventArgs());
                }
            };
            supersamplingCheckButton = new CheckButton("Supersampling");
            supersamplingCheckButton.Toggled += delegate
            {
                Module.SupersamplingEnabled =
                    supersamplingCheckButton.Active;
                postResizePanel.Sensitive =
                    !supersamplingCheckButton.Active;
                if (ModuleChanged != null) {
                    ModuleChanged(this, new EventArgs());
                }
            };

            Table postResizeTable = new Table(2, 1, false);
            postResizeTable.Attach(supersamplingCheckButton, 0, 1, 0, 1,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);
            postResizeTable.Attach(postResizePanel, 0, 1, 1, 2,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Fill | AttachOptions.Expand, 0, 0);

            postResizeFrame.Add(postResizeTable);
            postProcessingVBox.PackStart(postResizeFrame);

            // ---- smoothen ----

            postSmoothenEnabledCheckButton = new CheckButton("Enabled");
            postSmoothenEnabledCheckButton.Toggled += delegate
            {
                bool enabled = postSmoothenEnabledCheckButton.Active;
                if (!enabled) {
                    Module.PostSmoothen = null;
                } else if (Module.PostSmoothen == null) {
                    Module.PostSmoothen =
                        new HalftoneAlgorithm.Smoothen();
                }
                if (Module.PostSmoothen != null) {
                    postSmoothenRadiusSpinButton.Value = Module.PostSmoothen.Radius;
                }
                postSmoothenRadiusSpinButton.Sensitive = enabled;
                if (ModuleChanged != null) {
                    ModuleChanged(this, new EventArgs());
                }
            };
            postSmoothenRadiusSpinButton = new SpinButton(1, 20, 0.5);
            postSmoothenRadiusSpinButton.Changed += delegate
            {
                Module.PostSmoothen.Radius = postSmoothenRadiusSpinButton.Value;
                if (ModuleChanged != null) {
                    ModuleChanged(this, new EventArgs());
                }
            };
            Table postSmoothenTable = new Table(2, 2, false)
                { ColumnSpacing = 3, RowSpacing = 3, BorderWidth = 5 };
            postSmoothenTable.Attach(postSmoothenEnabledCheckButton, 0, 2, 0, 1,
                AttachOptions.Fill | AttachOptions.Expand,
                AttachOptions.Shrink, 0, 0);

            postSmoothenTable.Attach(new Label("Radius:") { Xalign = 0.0f },
                0, 1, 1, 2, AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
            postSmoothenTable.Attach(postSmoothenRadiusSpinButton, 1, 2, 1, 2,
                AttachOptions.Fill, AttachOptions.Shrink, 0, 0);
            postSmoothenFrame.Add(postSmoothenTable);

            postProcessingVBox.PackStart(postSmoothenFrame);

            AppendPage(postProcessingVBox, new Label("Post-processing"));

            Module = existingModule;

            ShowAll();

            Page = 1;
        }

        public class ResizePanel : Table {
            private HalftoneAlgorithm.Resize module;
            public HalftoneAlgorithm.Resize Module {
                get { return module; }
                set {
                    module = value;
                    bool moduleNotNull = module != null;
                    enabledCheckButton.Active = moduleNotNull;
                    resizeFactorSpinButton.Sensitive = moduleNotNull;
                    interpolationTypeComboBox.Sensitive = moduleNotNull;
                    if (moduleNotNull) {
                        resizeFactorSpinButton.Value = module.Factor;
                        interpolationTypeComboBox.Active =
                            interpolationTypeIndex(Module.Interpolation);
                    }
                    if (ModuleChanged != null) {
                        ModuleChanged(this, new EventArgs());
                    }
                }
            }
            public event EventHandler ModuleChanged;

            private CheckButton enabledCheckButton;
            private SpinButton resizeFactorSpinButton;
            private ComboBox interpolationTypeComboBox;
            private InterpolationTypeRecord[] interpolationTypes;

            public ResizePanel() : this(null) { }

            public ResizePanel(HalftoneAlgorithm.Resize existingModule)
                : base(3, 2, false)
            {
                ColumnSpacing = RowSpacing = 3;
                BorderWidth = 5;

                enabledCheckButton = new CheckButton("Enabled");
                enabledCheckButton.Toggled += delegate
                {
                    bool enabled = enabledCheckButton.Active;
                    if (enabled && (Module == null)) {
                        Module = new HalftoneAlgorithm.Resize();
                    }
                    if (!enabled && (Module != null)) {
                        Module = null;
                    }
                    if (module != null) {
                        resizeFactorSpinButton.Value = Module.Factor;
                        interpolationTypeComboBox.Active =
                            interpolationTypeIndex(Module.Interpolation);
                    }
                    resizeFactorSpinButton.Sensitive = enabled;
                    interpolationTypeComboBox.Sensitive = enabled;
                };
                resizeFactorSpinButton = new SpinButton(0.1, 10, 0.01);
                resizeFactorSpinButton.Changed += delegate
                {
                    Module.Factor = resizeFactorSpinButton.Value;
                    if (ModuleChanged != null) {
                        ModuleChanged(this, new EventArgs());
                    }
                };
                interpolationTypes = new InterpolationTypeRecord[] {
                    new InterpolationTypeRecord() { name = "Nearest neighbour",
                        type = HalftoneAlgorithm.Resize.InterpolationType.NearestNeighbour},
                    new InterpolationTypeRecord() { name = "Bilinear",
                        type = HalftoneAlgorithm.Resize.InterpolationType.Bilinear},
                    new InterpolationTypeRecord() { name = "Bicubic",
                        type = HalftoneAlgorithm.Resize.InterpolationType.Bicubic},
                    new InterpolationTypeRecord() { name = "Lanczos",
                        type = HalftoneAlgorithm.Resize.InterpolationType.Lanczos},
                };

                var interpolationTypeNames = from type in interpolationTypes select
                    type.name;

                interpolationTypeComboBox = new ComboBox(
                    new List<string>(interpolationTypeNames).ToArray());
                interpolationTypeComboBox.Changed += delegate
                {
                    HalftoneAlgorithm.Resize.InterpolationType value =
                    interpolationTypes[interpolationTypeComboBox.Active].type;
                    Module.Interpolation = value;
                    if (ModuleChanged != null) {
                        ModuleChanged(this, new EventArgs());
                    }
                };

                Module = existingModule;

                Attach(enabledCheckButton, 0, 2, 0, 1,
                    AttachOptions.Fill | AttachOptions.Expand,
                    AttachOptions.Shrink, 0, 0);

                Attach(new Label("Resize factor:") { Xalign = 0.0f },
                    0, 1, 1, 2, AttachOptions.Fill,
                    AttachOptions.Shrink, 0, 0);
                Attach(resizeFactorSpinButton, 1, 2, 1, 2,
                    AttachOptions.Fill,
                    AttachOptions.Shrink, 0, 0);

                Attach(new Label("Interpolation type:") { Xalign = 0.0f },
                    0, 1, 2, 3, AttachOptions.Fill,
                    AttachOptions.Shrink, 0, 0);
                Attach(interpolationTypeComboBox, 1, 2, 2, 3,
                    AttachOptions.Fill,
                    AttachOptions.Shrink, 0, 0);
                
                ShowAll();
            }

            private int interpolationTypeIndex(
                HalftoneAlgorithm.Resize.InterpolationType interpolationType)
            {
                return (from result in (interpolationTypes.Select(
                      (type, index) =>
                        new
                        {
                            Index = index,
                            Ok = type.type == interpolationType
                        }
                 ))
                 where result.Ok == true
                 select result.Index).First();
            }

            class InterpolationTypeRecord {
                public string name;
                public HalftoneAlgorithm.Resize.InterpolationType type;
            }
        }
    }
}
