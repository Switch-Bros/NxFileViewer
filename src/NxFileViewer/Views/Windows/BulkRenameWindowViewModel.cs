﻿using System;
using System.Collections.Generic;
using Emignatik.NxFileViewer.Services.FileRenaming;
using Emignatik.NxFileViewer.Services.FileRenaming.Models;
using Emignatik.NxFileViewer.Services.FileRenaming.Models.PatternParts.Application;
using Emignatik.NxFileViewer.Settings;
using Emignatik.NxFileViewer.Utils.MVVM;
using Emignatik.NxFileViewer.Utils.MVVM.Commands;

namespace Emignatik.NxFileViewer.Views.Windows;

public class BulkRenameWindowViewModel : WindowViewModelBase
{
    private readonly IFileRenamerService _fileRenamerService;
    private readonly INamingPatternsParser _namingPatternsParser;
    private readonly IAppSettingsManager _appSettingsManager;

    private string _inputDirectory;
    private string _patchPattern;
    private string _addonPattern;
    private List<ApplicationPatternPart>? _applicationPatternParts;
    private string? _applicationPatternError;

    public BulkRenameWindowViewModel(IFileRenamerService fileRenamerService, INamingPatternsParser namingPatternsParser, IAppSettingsManager appSettingsManager)
    {
        _fileRenamerService = fileRenamerService ?? throw new ArgumentNullException(nameof(fileRenamerService));
        _namingPatternsParser = namingPatternsParser ?? throw new ArgumentNullException(nameof(namingPatternsParser));
        _appSettingsManager = appSettingsManager ?? throw new ArgumentNullException(nameof(appSettingsManager));
        RenameCommand = new RelayCommand(Rename, CanRename);
        CancelCommand = new RelayCommand(Cancel);
        BrowseInputDirectoryCommand = new RelayCommand(OnBrowseInputDirectory);

        UpdateApplicationPatternParts();
    }

    public RelayCommand RenameCommand { get; }

    public RelayCommand CancelCommand { get; }

    public RelayCommand BrowseInputDirectoryCommand { get; }


    public string InputDirectory
    {
        get => _inputDirectory;
        set
        {
            _inputDirectory = value;
            NotifyPropertyChanged();
        }
    }

    public string ApplicationPattern
    {
        get => _appSettingsManager.Settings.ApplicationPattern;
        set
        {
            _appSettingsManager.Settings.ApplicationPattern = value;

            UpdateApplicationPatternParts();
            NotifyPropertyChanged();
        }
    }

    public string? ApplicationPatternError
    {
        get => _applicationPatternError;
        set
        {
            _applicationPatternError = value;
            NotifyPropertyChanged();
        }
    }

    private void UpdateApplicationPatternParts()
    {
        try
        {
            _applicationPatternParts = _namingPatternsParser.ParseApplicationPatterns(this.ApplicationPattern);
            ApplicationPatternError = null;
            _appSettingsManager.SaveSafe();
        }
        catch (Exception ex)
        {
            _applicationPatternParts = null;
            ApplicationPatternError = ex.Message;
        }

        RenameCommand.TriggerCanExecuteChanged();
    }

    public string PatchPattern
    {
        get => _patchPattern;
        set
        {
            _patchPattern = value;
            NotifyPropertyChanged();
        }
    }

    public string AddonPattern
    {
        get => _addonPattern;
        set
        {
            _addonPattern = value;
            NotifyPropertyChanged();
        }
    }

    private void OnBrowseInputDirectory()
    {
        //TODO: à implémenter
    }

    private void Cancel()
    {
        this.Window?.Close();
    }

    private void Rename()
    {
        //TODO: exposer tous les paramètres
        try
        {
            var namingPatterns = new NamingPatterns
            {
                ApplicationPattern = _applicationPatternParts!,
            };

            _fileRenamerService.RenameFromDirectory(InputDirectory, namingPatterns, new[] { ".nsp", ".nsz", ".xci", ".xcz" }, true);
        }
        catch (Exception ex)
        {
            //TODO: gérer l'exception
        }
    }

    private bool CanRename()
    {
        return _applicationPatternParts != null;
    }

}