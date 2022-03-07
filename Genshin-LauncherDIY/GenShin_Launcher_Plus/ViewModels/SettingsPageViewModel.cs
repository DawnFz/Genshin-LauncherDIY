﻿using GenShin_Launcher_Plus.Core;
using GenShin_Launcher_Plus.Models;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.VisualBasic.Devices;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace GenShin_Launcher_Plus.ViewModels
{
    internal class SettingsPageViewModel : ObservableObject
    {
        //转换文件列表
        private string[] globalfiles = new string[]
        { "GenshinImpact_Data/app.info",
          "GenshinImpact_Data/globalgamemanagers",
          "GenshinImpact_Data/globalgamemanagers.assets",
          "GenshinImpact_Data/globalgamemanagers.assets.resS",
          "GenshinImpact_Data/upload_crash.exe",
          "GenshinImpact_Data/Managed/Metadata/global-metadata.dat" ,
          "GenshinImpact_Data/Native/Data/Metadata/global-metadata.dat",
          "GenshinImpact_Data/Native/UserAssembly.dll",
          "GenshinImpact_Data/Native/UserAssembly.exp",
          "GenshinImpact_Data/Native/UserAssembly.lib",
          "GenshinImpact_Data/Plugins/cri_mana_vpx.dll",
          "GenshinImpact_Data/Plugins/cri_vip_unity_pc.dll",
          "GenshinImpact_Data/Plugins/cri_ware_unity.dll",
          "GenshinImpact_Data/Plugins/d3dcompiler_43.dll",
          "GenshinImpact_Data/Plugins/d3dcompiler_47.dll",
          "GenshinImpact_Data/Plugins/hdiffz.dll",
          "GenshinImpact_Data/Plugins/hpatchz.dll",
          "GenshinImpact_Data/Plugins/mihoyonet.dll",
          "GenshinImpact_Data/Plugins/Mmoron.dll",
          "GenshinImpact_Data/Plugins/MTBenchmark_Windows.dll",
          "GenshinImpact_Data/Plugins/NamedPipeClient.dll",
          "GenshinImpact_Data/Plugins/UnityNativeChromaSDK.dll",
          "GenshinImpact_Data/Plugins/UnityNativeChromaSDK3.dll",
          "GenshinImpact_Data/Plugins/xlua.dll",
          "GenshinImpact_Data/StreamingAssets/20527480.blk",
          "Audio_Chinese_pkg_version",
          "pkg_version",
          "UnityPlayer.dll",
          "GenshinImpact.exe"
};

        private string[] cnfiles = new string[]
        { "YuanShen_Data/app.info",
          "YuanShen_Data/globalgamemanagers",
          "YuanShen_Data/globalgamemanagers.assets",
          "YuanShen_Data/globalgamemanagers.assets.resS",
          "YuanShen_Data/upload_crash.exe",
          "YuanShen_Data/Managed/Metadata/global-metadata.dat" ,
          "YuanShen_Data/Native/Data/Metadata/global-metadata.dat",
          "YuanShen_Data/Native/UserAssembly.dll",
          "YuanShen_Data/Native/UserAssembly.exp",
          "YuanShen_Data/Native/UserAssembly.lib",
          "YuanShen_Data/Plugins/cri_mana_vpx.dll",
          "YuanShen_Data/Plugins/cri_vip_unity_pc.dll",
          "YuanShen_Data/Plugins/cri_ware_unity.dll",
          "YuanShen_Data/Plugins/d3dcompiler_43.dll",
          "YuanShen_Data/Plugins/d3dcompiler_47.dll",
          "YuanShen_Data/Plugins/hdiffz.dll",
          "YuanShen_Data/Plugins/hpatchz.dll",
          "YuanShen_Data/Plugins/mihoyonet.dll",
          "YuanShen_Data/Plugins/Mmoron.dll",
          "YuanShen_Data/Plugins/MTBenchmark_Windows.dll",
          "YuanShen_Data/Plugins/NamedPipeClient.dll",
          "YuanShen_Data/Plugins/UnityNativeChromaSDK.dll",
          "YuanShen_Data/Plugins/UnityNativeChromaSDK3.dll",
          "YuanShen_Data/Plugins/xlua.dll",
          "YuanShen_Data/StreamingAssets/20527480.blk",
          "Audio_Chinese_pkg_version",
          "pkg_version",
          "UnityPlayer.dll",
          "YuanShen.exe"
        };

        //

        //构造器
        private IDialogCoordinator dialogCoordinator;
        public SettingsPageViewModel(IDialogCoordinator instance)
        {
            IniModel = new SettingsIniModel();
            dialogCoordinator = instance;
            SettingsPageCreated();
            CreateDisplaySizeList();
            CreateGamePortList();
            CreateGameWindowModeList();
            ReadUserList();
            SaveSettingsCommand = new RelayCommand(SaveSettings);
            DeleteUserCommand = new RelayCommand(DeleteUser);
            ChooseGamePathCommand = new RelayCommand(ChooseGamePath);
            ChooseUnlockFpsCommand = new RelayCommand(ChooseUnlockFps);
            GameFileConvertCommand = new RelayCommand(GameFileConvert);
            Auto21x9Command = new RelayCommand(Auto21x9);
            ThisPageRemoveCommand = new RelayCommand(ThisPageRemove);
        }

        //保存状态
        private string _SettingsTitle = "设置";
        public string SettingsTitle
        {
            get => _SettingsTitle;
            set => SetProperty(ref _SettingsTitle, value);
        }
        private string _SettingTitleColor = "#FF272727";
        public string SettingTitleColor
        {
            get => _SettingTitleColor;
            set => SetProperty(ref _SettingTitleColor, value);
        }
        private void DelaySaveButtonTitle()
        {
            Task task = new(() =>
            {
                SettingsTitle = "设置  [保存成功]";
                SettingTitleColor = "#FF008C02";
                Thread.Sleep(1500);
                SettingsTitle = "设置";
                SettingTitleColor = "#FF272727";
            });
            task.Start();
        }
        //设置界面UI刷新绑定数据
        private string _Width;
        public string Width { get => _Width; set => SetProperty(ref _Width, value); }
        private string _Height;
        public string Height { get => _Height; set => SetProperty(ref _Height, value); }
        private bool _isUnFPS;
        public bool isUnFPS { get => _isUnFPS; set => SetProperty(ref _isUnFPS, value); }

        //选中分辨率的索引
        private int _DisplaySelectedIndex = -1;
        public int DisplaySelectedIndex
        {
            get => _DisplaySelectedIndex;
            set
            {
                switch (value)
                {
                    case 0:
                        Width = "3840";
                        Height = "2160";
                        break;
                    case 1:
                        Width = "2560";
                        Height = "1080";
                        break;
                    case 2:
                        Width = "1920";
                        Height = "1080";
                        break;
                    case 3:
                        Width = "1600";
                        Height = "900";
                        break;
                    case 4:
                        Width = "1360";
                        Height = "768";
                        break;
                    case 5:
                        Width = "1280";
                        Height = "1024";
                        break;
                    case 6:
                        Width = "1280";
                        Height = "720";
                        break;
                    case 7:
                        Width = Convert.ToString(SystemParameters.PrimaryScreenWidth);
                        Height = Convert.ToString(SystemParameters.PrimaryScreenHeight);
                        break;
                    default:
                        break;
                }
                IniModel.Width = Width;
                IniModel.Height = Height;
            }
        }
        //存放设置属性的实体类
        private SettingsIniModel _IniModel;
        public SettingsIniModel IniModel
        {
            get => _IniModel;
            set => SetProperty(ref _IniModel, value);

        }

        //转换时的日志列表
        private string _GameSwitchLog = "PKG转换文件度盘下载链接，密码：etxd\r\nhttps://pan.baidu.com/s/1-5zQoVfE7ImdXrn8OInKqg\r\n";
        public string GameSwitchLog
        {
            get => _GameSwitchLog;
            set => SetProperty(ref _GameSwitchLog, value);
        }

        //转换时的控件状态
        private string _PageUiStatus = "true";
        public string PageUiStatus
        {
            get => _PageUiStatus;
            set => SetProperty(ref _PageUiStatus, value);
        }

        //转换状态
        private string _TimeStatus = "当前状态：无状态";
        public string TimeStatus
        {
            get => _TimeStatus;
            set => SetProperty(ref _TimeStatus, value);
        }
        public List<DisplaySizeListModel> DisplaySizeLists { get; set; }
        private void CreateDisplaySizeList()
        {
            DisplaySizeLists = new List<DisplaySizeListModel>
            {
                new DisplaySizeListModel { DisplaySize = "3840 × 2160  | 16:9" },
                new DisplaySizeListModel { DisplaySize = "2560 × 1080  | 21:9" },
                new DisplaySizeListModel { DisplaySize = "1920 × 1080  | 16:9" },
                new DisplaySizeListModel { DisplaySize = "1600 × 900    | 16:9" },
                new DisplaySizeListModel { DisplaySize = "1360 × 768    | 16:9" },
                new DisplaySizeListModel { DisplaySize = "1280 × 1024  |  4:3" },
                new DisplaySizeListModel { DisplaySize = "1280 × 720    | 16:9" },
                new DisplaySizeListModel{ DisplaySize = "自适应全屏" },
            };
        }

        //用户列表
        public List<UserListModel> _UserLists;
        public List<UserListModel> UserLists
        {
            get => _UserLists;
            set => SetProperty(ref _UserLists, value);
        }
        private void ReadUserList()
        {
            UserLists = new List<UserListModel>();
            DirectoryInfo TheFolder = new(@"UserData");
            foreach (FileInfo NextFile in TheFolder.GetFiles())
            {
                UserLists.Add(new UserListModel { UserName = NextFile.Name });
            }
        }

        //游戏客户端列表
        private List<GamePortListModel> _GamePortLists;
        public List<GamePortListModel> GamePortLists
        {
            get => _GamePortLists;
            set => SetProperty(ref _GamePortLists, value);
        }
        private void CreateGamePortList()
        {
            GamePortLists = new List<GamePortListModel>
            {
                new GamePortListModel { GamePort = "官方服务器" },
                new GamePortListModel { GamePort = "哔哩哔哩服" },
                new GamePortListModel { GamePort = "国际服务器" }
            };
        }

        //游戏窗口模式列表
        private List<GameWindowModeListModel> _GameWindowModeList;
        public List<GameWindowModeListModel> GameWindowModeList
        {
            get => _GameWindowModeList;
            set => SetProperty(ref _GameWindowModeList, value);
        }
        private void CreateGameWindowModeList()
        {
            GameWindowModeList = new();
            GameWindowModeList.Add(new GameWindowModeListModel { GameWindowMode = "窗口启动" });
            GameWindowModeList.Add(new GameWindowModeListModel { GameWindowMode = "全屏启动" });
        }

        //选择游戏路径的命令
        public ICommand ChooseGamePathCommand { get; set; }
        private void ChooseGamePath()
        {
            CommonOpenFileDialog dialog = new("请选择原神游戏本体所在文件夹");
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                IniModel.GamePath = dialog.FileName;
            }

        }

        //开始转换显示的等待条
        private string _ProgressBar = "Hidden";
        public string ProgressBar
        {
            get => _ProgressBar;
            set => SetProperty(ref _ProgressBar, value);
        }

        //选择解锁FPS的指令
        public ICommand ChooseUnlockFpsCommand { get; set; }
        private async void ChooseUnlockFps()
        {
            if (isUnFPS)
            {
                if ((await dialogCoordinator.ShowMessageAsync(this, "超级警告", "此操作涉及修改游戏客户端进程，目前不知道确切会不会出现封号风险，出现问题请自行承担后果！如之前没使用过UnlockFPS的建议不要使用！按下同意代表使用本功能后的一切后果由自己承担！怕就不要用，用就不要怕！【注意：启用本功能后拉起游戏会慢一点，为正常现象】", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() { AffirmativeButtonText = "取消", NegativeButtonText = "同意" })) != MessageDialogResult.Affirmative)
                {
                    isUnFPS = true;
                    IniModel.isUnFPS = isUnFPS;
                }
                else
                {
                    isUnFPS = false;
                    IniModel.isUnFPS = isUnFPS;
                }
            }
        }

        //删除账号的命令
        public ICommand DeleteUserCommand { get; set; }
        private async void DeleteUser()
        {

            if (IniModel.SwitchUser != "" && IniModel.SwitchUser != null)
            {
                if ((await dialogCoordinator.ShowMessageAsync(this, "警告", $"您确定删除账号：{IniModel.SwitchUser} 吗？！", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() { AffirmativeButtonText = "取消", NegativeButtonText = "删除" })) != MessageDialogResult.Affirmative)
                {
                    File.Delete(Path.Combine(@"UserData", IniModel.SwitchUser));
                    ReadUserList();
                }
            }
            else
            {
                await dialogCoordinator.ShowMessageAsync(this, "错误", "请选中一个账号再使用本功能！", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
            }
        }

        //保存设置的命令
        public ICommand SaveSettingsCommand { get; set; }
        private async void SaveSettings()
        {
            if (IniModel.SwitchUser != null && IniModel.SwitchUser != "")
            {
                IniControl.SwitchUser = IniModel.SwitchUser;
                MainBase.noab.SwitchUser = $"账号：{IniModel.SwitchUser}";
                MainBase.noab.IsSwitchUser = "Visible";
                RegistryControl registryControl = new();
                registryControl.SetToRegedit(IniModel.SwitchUser);
            }
            if (IniModel.GamePath != "" && File.Exists(Path.Combine(IniModel.GamePath, "Yuanshen.exe")) || File.Exists(Path.Combine(IniModel.GamePath, "GenshinImpact.exe")))
            {
                IniControl.GamePath = IniModel.GamePath;
            }
            else
            {
                await dialogCoordinator.ShowMessageAsync(this, "错误", "路径为空或路径内不含游戏客户端，请重新选择！", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
                return;
            }
            IniControl.Width = Width;
            IniControl.Height = Height;
            IniControl.isUnFPS = IniModel.isUnFPS;
            IniControl.MaxFps = IniModel.MaxFps;
            IniControl.isPopup = IniModel.isPopup;
            IniControl.isMainGridHide = IniModel.isMainGridHide;
            IniControl.isWebBg = IniModel.isWebBg;
            IniControl.FullSize = IniModel.FullSize;
            IniControl.UserXunkongWallpaper = IniModel.UseXunkongWallpaper;
            if (File.Exists(Path.Combine(IniControl.GamePath, "config.ini")) == true)
            {
                switch (IniModel.isMihoyo)
                {
                    case 0:
                        IniControl.Cps = "pcadbdpz";
                        IniControl.Channel = 1;
                        IniControl.Sub_channel = 1;
                        if (File.Exists(Path.Combine(IniModel.GamePath, "YuanShen_Data/Plugins/PCGameSDK.dll")))
                            File.Delete(Path.Combine(IniModel.GamePath, "YuanShen_Data/Plugins/PCGameSDK.dll"));
                        MainBase.noab.SwitchPort = "客户端：官方服务器";
                        break;
                    case 1:
                        IniControl.Cps = "bilibili";
                        IniControl.Channel = 14;
                        IniControl.Sub_channel = 0;
                        if (!File.Exists(Path.Combine(IniModel.GamePath, "YuanShen_Data/Plugins/PCGameSDK.dll")))
                        {
                            FilesControl utils = new();
                            try
                            {
                                utils.FileWriter("StaticRes/mihoyosdk.dll", Path.Combine(IniModel.GamePath, "YuanShen_Data/Plugins/PCGameSDK.dll"));
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }
                        MainBase.noab.SwitchPort = "客户端：哔哩哔哩服";
                        break;
                    case 2:
                        IniControl.Cps = "mihoyo";
                        IniControl.Channel = 1;
                        IniControl.Sub_channel = 0;
                        if (File.Exists(Path.Combine(IniModel.GamePath, "GenshinImpact_Data/Plugins/PCGameSDK.dll")))
                            File.Delete(Path.Combine(IniModel.GamePath, "GenshinImpact_Data/Plugins/PCGameSDK.dll"));
                        MainBase.noab.SwitchPort = "客户端：通用国际服";
                        break;
                    default:
                        break;
                }
            }
            DelaySaveButtonTitle();
            MainBase.noab.MainPagesIndex = 0;
        }

        //读取判断游戏客户端信息
        private void ReadGameConfig()
        {
            if (File.Exists(Path.Combine(IniControl.GamePath, "config.ini")))
            {
                if (IniControl.Cps == "pcadbdpz")
                { IniModel.isMihoyo = 0; }
                else if (IniControl.Cps == "bilibili")
                { IniModel.isMihoyo = 1; }
                else if (IniControl.Cps == "mihoyo")
                { IniModel.isMihoyo = 2; }
                else
                { IniModel.isMihoyo = 3; }
            }
            else
            { IniModel.isMihoyo = 3; }
        }

        //被创建时从Setting.ini文件读取给IniModel对象赋值
        private void SettingsPageCreated()
        {
            IniModel.GamePath = IniControl.GamePath;
            IniModel.Width = IniControl.Width;
            IniModel.Height = IniControl.Height;
            IniModel.isUnFPS = IniControl.isUnFPS;
            IniModel.MaxFps = IniControl.MaxFps;
            IniModel.GamePath = IniControl.GamePath;
            IniModel.isPopup = IniControl.isPopup;
            IniModel.isMainGridHide = IniControl.isMainGridHide;
            IniModel.isWebBg = IniControl.isWebBg;
            IniModel.FullSize = IniControl.FullSize;
            Width = IniModel.Width;
            Height = IniModel.Height;
            IniModel.UseXunkongWallpaper = IniControl.UserXunkongWallpaper;
            ReadGameConfig();
        }

        //自动取比例
        public ICommand Auto21x9Command { get; set; }
        private async void Auto21x9()
        {
            if (Height == "" && Width != "")
            {
                int x = Convert.ToInt32(Width);
                int y = x * 9 / 21;
                Width = Convert.ToString(x);
                Height = Convert.ToString(y);
            }
            else if (Width == "" && Height != "")
            {
                int y = Convert.ToInt32(Height);
                int x = y * 21 / 9;
                Width = Convert.ToString(x);
                Height = Convert.ToString(y);
            }
            else
            {
                await dialogCoordinator.ShowMessageAsync(this, "提醒", "在上面随便一个框填上想要的宽或者高另一个框留空使用本按钮自动取21:9比例分辨率", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
            }
        }


        //关闭设置页面
        public ICommand ThisPageRemoveCommand { get; set; }
        private void ThisPageRemove()
        {
            MainBase.noab.MainPagesIndex = 0;
        }

        //转换国际服及转换国服绑定命令
        public ICommand GameFileConvertCommand { get; set; }
        private void GameFileConvert()
        {
            if (!CheckControl.IsFileOpen(Path.Combine(IniControl.GamePath, "Yuanshen.exe")) && !CheckControl.IsFileOpen(Path.Combine(IniControl.GamePath, "GenshinImpact.exe")))
            {

                Task start = new(async () =>
                {
                    if ((await dialogCoordinator.ShowMessageAsync(this, "警告！！", "转换或还原将会执行重命名，替换，删除等操作修改客户端文件，该过程大概率会触发杀软报毒！为了防止客户端损坏导致不完整，执行前检查杀软（包括 Windows Defender）是否完全关闭或将本启动器加入白名单，并检查游戏是否彻底关闭，否则可能将导致客户端文件缺失！！\r\n\r\n提示：如游戏大版本更新时请执行还原转换为国内服使用游戏自带启动器更新！", MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings() { AffirmativeButtonText = "取消转换", NegativeButtonText = "确定转换" })) != MessageDialogResult.Affirmative)
                    {
                        PageUiStatus = "false";
                        ProgressBar = "Visible";
                        //判断客户端
                        string port = JudgeGamePort();
                        //判断Pkg是否正常
                        if (port == "YuanShen")
                        {
                            if (!CheckFileIntegrity(IniControl.GamePath, cnfiles, 1, ".bak"))
                            {
                                //没备份文件
                                if (Directory.Exists(@"GlobalFile"))
                                {
                                    if (JudgePkgVer("GlobalFile"))
                                    {
                                        if (CheckFileIntegrity(@"GlobalFile", globalfiles, 0))
                                        {
                                            await GlobalMoveFile();
                                        }
                                    }
                                    else
                                    {
                                        DirectoryInfo di = new(@"GlobalFile");
                                        di.Delete(true);
                                    }
                                }
                                else if (File.Exists(@"GlobalFile.pkg"))
                                {
                                    TimeStatus = "当前状态：正在解压PKG资源包";
                                    //解压Pkg
                                    if (FilesControl.UnZip("GlobalFile.pkg", @""))
                                    {
                                        if (JudgePkgVer("GlobalFile"))
                                        {
                                            await GlobalMoveFile();
                                        }
                                        else
                                        {
                                            DirectoryInfo di = new(@"GlobalFile");
                                            di.Delete(true);
                                            TimeStatus = "当前状态：Pkg有新版本";
                                        }
                                    }
                                    //解压失败
                                    else
                                    {
                                        TimeStatus = "当前状态：解压失败，请检查";
                                        GameSwitchLog += "没有找到资源[GlobalFile.pkg]或解压失败，请检查Pkg文件是否和本应用处于同一目录\r\n";
                                        await dialogCoordinator.ShowMessageAsync(this, "错误", "PKG文件不存在或解压失败", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
                                    }
                                }
                                else
                                {
                                    TimeStatus = "当前状态：请检查Pkg文件";
                                    GameSwitchLog += "没有找到资源[GlobalFile.pkg]，请检查Pkg文件是否和本应用处于同一目录\r\n";
                                    await dialogCoordinator.ShowMessageAsync(this, "错误", "PKG文件不存在", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
                                }
                            }
                            else
                            {
                                await ReGlobalGame();
                            }
                        }
                        else
                        {
                            if (!CheckFileIntegrity(IniControl.GamePath, globalfiles, 1, ".bak"))
                            {
                                //没备份文件
                                if (Directory.Exists(@"CnFile"))
                                {
                                    if (JudgePkgVer("CnFile"))
                                    {
                                        if (CheckFileIntegrity(@"CnFile", cnfiles, 0))
                                        {
                                            await CnMoveFile();
                                        }
                                    }
                                    else
                                    {
                                        DirectoryInfo di = new(@"CnFile");
                                        di.Delete(true);
                                    }
                                }
                                else if (File.Exists(@"CnFile.pkg"))
                                {
                                    TimeStatus = "当前状态：正在解压PKG资源包";
                                    //解压Pkg
                                    if (FilesControl.UnZip("CnFile.pkg", @""))
                                    {
                                        if (JudgePkgVer("CnFile"))
                                        {
                                            await CnMoveFile();
                                        }
                                        else
                                        {
                                            DirectoryInfo di = new(@"CnFile");
                                            di.Delete(true);
                                            TimeStatus = "当前状态：Pkg有新版本";
                                        }
                                    }
                                    //解压失败
                                    else
                                    {
                                        TimeStatus = "当前状态：解压失败，请检查";
                                        GameSwitchLog += "没有找到资源[CnFile.pkg]或解压失败，请检查Pkg文件是否和本应用处于同一目录\r\n";
                                        await dialogCoordinator.ShowMessageAsync(this, "错误", "PKG文件不存在或解压失败", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
                                    }
                                }
                                else
                                {
                                    TimeStatus = "当前状态：请检查Pkg文件";
                                    GameSwitchLog += "没有找到资源[CnFile.pkg]，请检查Pkg文件是否和本应用处于同一目录\r\n";
                                    await dialogCoordinator.ShowMessageAsync(this, "错误", "PKG文件不存在", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
                                }
                            }
                            else
                            {
                                await ReCnGame();
                            }
                        }
                        ProgressBar = "Hidden";
                        PageUiStatus = "true";
                        SaveSettings();
                    }
                });
                start.Start();
            }
            else
            {
                dialogCoordinator.ShowMessageAsync(this, "错误", "请先关闭游戏再执行转换操作，如确定游戏已经完全关闭还是弹此提示请重启电脑再试或联系开发者！", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
            }
        }

        //转换国际服及转换国服核心逻辑-判断客户端
        private string JudgeGamePort()
        {
            if (File.Exists(Path.Combine(IniControl.GamePath, "YuanShen.exe")))
            {
                return "YuanShen";
            }
            else if (File.Exists(Path.Combine(IniControl.GamePath, "GenshinImpact.exe")))
            {
                return "GenshinImpact";
            }
            else
            {
                return null;
            }
        }

        //转换国际服及转换国服核心逻辑-判断PKG文件版本
        private bool JudgePkgVer(string GamePort)
        {
            string pkgfile = FilesControl.MiddleText(FilesControl.ReadHTML("https://www.cnblogs.com/DawnFz/p/7271382.html", "UTF-8"), "[$pkg$]", "[#pkg#]");
            if (!File.Exists($"{GamePort}/{pkgfile}"))
            {
                dialogCoordinator.ShowMessageAsync(this, "提示", "国服转换包有新版本：" + pkgfile + "\r\n访问密码：etxd", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
                ProcessStartInfo info = new()
                {
                    FileName = "https://pan.baidu.com/s/1-5zQoVfE7ImdXrn8OInKqg",
                    UseShellExecute = true,
                };
                Process.Start(info);
                return false;
            }
            else
            {
                return true;
            }
        }

        //遍历判断文件是否存在
        private bool CheckFileIntegrity(string dirpath, string[] filepath, int len, string postfix = "")
        {
            bool notError = true;
            for (int i = 0; i < filepath.Length - len; i++)
            {
                if (File.Exists(Path.Combine(dirpath, filepath[i] + postfix)) == false)
                {
                    GameSwitchLog += filepath[i] + postfix + "文件不存在，启动器将尝试下一步操作，若无反应请重新下载资源文件！\n";
                    notError = false;
                    break;
                }
                GameSwitchLog += filepath[i] + postfix + "存在\n";
            }
            return notError;
        }

        //国内转国际
        private async Task GlobalMoveFile()
        {
            Computer redir = new();
            TimeStatus = "当前状态：正在备份原文件";
            for (int a = 0; a < cnfiles.Length; a++)
            {
                String newFileName = Path.GetFileNameWithoutExtension(Path.Combine(IniControl.GamePath, cnfiles[a])) + Path.GetExtension(Path.Combine(IniControl.GamePath, cnfiles[a]));
                if (File.Exists(Path.Combine(IniControl.GamePath, cnfiles[a])) == true)
                {
                    try
                    {
                        redir.FileSystem.RenameFile(Path.Combine(IniControl.GamePath, cnfiles[a]), newFileName + ".bak");
                    }
                    catch (Exception ex)
                    {

                        GameSwitchLog += newFileName + "备份失败：原因：";
                        GameSwitchLog += ex.Message + "\n\n";
                    }

                    GameSwitchLog += newFileName + "备份成功\n";
                }
                else
                {
                    GameSwitchLog += newFileName + "文件不存在，备份失败，跳过\n";
                }
            }
            TimeStatus = "当前状态：开始替换资源";
            redir.FileSystem.RenameDirectory(Path.Combine(IniControl.GamePath, "YuanShen_Data"), "GenshinImpact_Data");
            for (int i = 0; i < globalfiles.Length; i++)
            {
                File.Copy(Path.Combine(@"GlobalFile", globalfiles[i]), Path.Combine(IniControl.GamePath, globalfiles[i]), true);
                GameSwitchLog += globalfiles[i] + "替换成功\n";
            };
            IniModel.isMihoyo = 2;
            TimeStatus = "当前状态：无状态";
            await dialogCoordinator.ShowMessageAsync(this, "提示", "转换完毕，按下确定自动保存，尽情享受吧！~", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
        }

        //国际转国内
        private async Task CnMoveFile()
        {
            Computer redir = new();
            TimeStatus = "当前状态：正在备份原文件";
            for (int a = 0; a < globalfiles.Length; a++)
            {
                String newFileName = Path.GetFileNameWithoutExtension(Path.Combine(IniControl.GamePath, globalfiles[a])) + Path.GetExtension(Path.Combine(IniControl.GamePath, globalfiles[a]));
                if (File.Exists(Path.Combine(IniControl.GamePath, globalfiles[a])) == true)
                {
                    try
                    {
                        redir.FileSystem.RenameFile(Path.Combine(IniControl.GamePath, globalfiles[a]), newFileName + ".bak");
                    }
                    catch (Exception ex)
                    {

                        GameSwitchLog += newFileName + "备份失败：原因：";
                        GameSwitchLog += ex.Message + "\n\n";
                    }

                    GameSwitchLog += newFileName + "备份成功\n";
                }
                else
                {
                    GameSwitchLog += newFileName + "文件不存在，备份失败，跳过\n";
                }
            }
            TimeStatus = "当前状态：开始替换资源";
            redir.FileSystem.RenameDirectory(Path.Combine(IniControl.GamePath, "GenshinImpact_Data"), "YuanShen_Data");
            for (int i = 0; i < cnfiles.Length; i++)
            {
                File.Copy(Path.Combine(@"CnFile", cnfiles[i]), Path.Combine(IniControl.GamePath, cnfiles[i]), true);
                GameSwitchLog += cnfiles[i] + "替换成功\n";
            };
            IniModel.isMihoyo = 0;
            TimeStatus = "当前状态：无状态";
            await dialogCoordinator.ShowMessageAsync(this, "提示", "转换完毕，按下确定自动保存，尽情享受吧！~", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
        }
        //还原
        private async Task ReCnGame()
        {
            Computer redir = new();
            TimeStatus = "当前状态：清理现存文件";
            for (int i = 0; i < globalfiles.Length; i++)
            {
                if (File.Exists(Path.Combine(IniControl.GamePath, globalfiles[i])) == true)
                {
                    File.Delete(Path.Combine(IniControl.GamePath, globalfiles[i]));
                    GameSwitchLog += globalfiles[i] + "清理完毕\n";
                }
                else
                {
                    GameSwitchLog += globalfiles[i] + "文件不存在，已跳过\n";
                }
            }
            TimeStatus = "当前状态：正在还原文件";
            redir.FileSystem.RenameDirectory(Path.Combine(IniControl.GamePath, "GenshinImpact_Data"), "YuanShen_Data");
            int whole = 0, success = 0;
            for (int a = 0; a < cnfiles.Length; a++)
            {
                string newFileName = Path.GetFileNameWithoutExtension(cnfiles[a]) + Path.GetExtension(cnfiles[a]);
                if (File.Exists(Path.Combine(IniControl.GamePath, cnfiles[a] + ".bak")) == true)
                {
                    redir.FileSystem.RenameFile(Path.Combine(IniControl.GamePath, cnfiles[a] + ".bak"), newFileName);
                    GameSwitchLog += cnfiles[a] + "还原成功\n";
                    success++;
                }
                else
                {

                    GameSwitchLog += cnfiles[a] + "不存在，跳过还原\n";
                    whole++;
                }
            }
            TimeStatus = "当前状态：无状态";
            IniModel.isMihoyo = 0;
            await dialogCoordinator.ShowMessageAsync(this, "提示", "还原完毕，本次还原成功" + success + "个文件，失败或缺失" + whole + "个文件，按下确定自动保存，尽情享受吧！~", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
        }

        private async Task ReGlobalGame()
        {
            Computer redir = new();
            TimeStatus = "当前状态：清理现存文件";
            for (int i = 0; i < cnfiles.Length; i++)
            {
                if (File.Exists(Path.Combine(IniControl.GamePath, cnfiles[i])) == true)
                {
                    File.Delete(Path.Combine(IniControl.GamePath, cnfiles[i]));
                    GameSwitchLog += cnfiles[i] + "清理完毕\n";
                }
                else
                {
                    GameSwitchLog += cnfiles[i] + "文件不存在，已跳过\n";
                }
            }
            TimeStatus = "当前状态：正在还原文件";
            redir.FileSystem.RenameDirectory(Path.Combine(IniControl.GamePath, "YuanShen_Data"), "GenshinImpact_Data");
            int whole = 0, success = 0;
            for (int a = 0; a < globalfiles.Length; a++)
            {
                string newFileName = Path.GetFileNameWithoutExtension(globalfiles[a]) + Path.GetExtension(globalfiles[a]);
                if (File.Exists(Path.Combine(IniControl.GamePath, globalfiles[a] + ".bak")) == true)
                {
                    redir.FileSystem.RenameFile(Path.Combine(IniControl.GamePath, globalfiles[a] + ".bak"), newFileName);
                    GameSwitchLog += globalfiles[a] + "还原成功\n";
                    success++;
                }
                else
                {

                    GameSwitchLog += globalfiles[a] + "不存在，跳过还原\n";
                    whole++;
                }
            }
            TimeStatus = "当前状态：无状态";
            IniModel.isMihoyo = 2;
            await dialogCoordinator.ShowMessageAsync(this, "提示", "还原完毕，本次还原成功" + success + "个文件，失败或缺失" + whole + "个文件，按下确定自动保存，尽情享受吧！", MessageDialogStyle.Affirmative, new MetroDialogSettings() { AffirmativeButtonText = "确定" });
        }
    }
}
