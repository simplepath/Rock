(function ($) {
  'use strict';
  window.Rock = window.Rock || {};
  Rock.controls = Rock.controls || {};

  Rock.controls.assetStorageSystemBrowser = (function () {
    var exports;

    exports = {
      initialize: function (options) {
        var self = this;

        var $assetBrowser = $('#'+ options.controlId);
        var $folderTreeView = $assetBrowser.find('.js-folder-treeview .treeview');
        var $selectFolder = $assetBrowser.find('.js-selectfolder');
        var $assetStorageId = $assetBrowser.find('.js-assetstorage-id');
        var $treePort = $assetBrowser.find('.js-treeviewport');

        var $createFolder = $assetBrowser.find('.js-createfolder');
        var $createFolderDiv = $assetBrowser.find('.js-createfolder-div');
        var $createFolderInput = $assetBrowser.find('.js-createfolder-input');
        var $createFolderCancel = $assetBrowser.find('.js-createfolder-cancel');

        var $deleteFolder = $assetBrowser.find('.js-deletefolder')
        var $fileCheckboxes = $assetBrowser.find('.js-checkbox');

        var $renameFile = $assetBrowser.find('.js-renamefile');
        var $renameFileDiv = $assetBrowser.find('.js-renamefile-div');
        var $renameFileInput = $assetBrowser.find('.js-renamefile-input');
        var $renameFileCancel = $assetBrowser.find('.js-renamefile-cancel');

        if ($folderTreeView.length == 0) {
          return;
        }

        var folderTreeData = $folderTreeView.data('rockTree');
        debugger

        if (!folderTreeData) {
          var selectedFolders = $selectFolder.text().split(',');

          $folderTreeView.rockTree({
            selectedIds: selectedFolders
          });

          var treePortIScroll = new IScroll($treePort[0], {
            mouseWheel: true,
            indicators: {
              el: '.js-treetrack',
              interactive: true,
              resize: false,
              listenY: true,
              listenX: false
            },
            click: false,
            preventDefaultException: { tagName: /.*/ }
          });

          $folderTreeView.on('rockTree:expand rockTree:collapse rockTree:dataBound rockTree:rendered', function (evt) {
            if (treePortIScroll) {
              treePortIScroll.refresh();
            }
          });
        }

        $folderTreeView.off('rockTree:selected');
        $folderTreeView.on('rockTree:selected', function (e, data) {
          var relativeFolderPath = data;
          var postbackArg;
          var previousStorageId = $assetStorageId.text();
          if (data.endsWith("/")) {
            $selectFolder.text(data);
            postbackArg = 'folder-selected:' + relativeFolderPath.replace(/\\/g, "/") + ',previous-asset:' + previousStorageId;
          }
          else {
            $assetStorageId.text(data);
            $selectFolder.text('');
            postbackArg = 'asset-selected:' + data + ',previous-asset:' + previousStorageId + ',folder-selected:';
          }

          var jsPostback = "javascript:__doPostBack('" + options.filesUpdatePanelId + "','" +  postbackArg+ "');"

          window.location = jsPostback;
        });

        // Some buttons are only active if one file is selected.
        $fileCheckboxes.off('click').on('click', function () {
          var n = $fileCheckboxes.filter(':checked').length;
          if (n != 1) {
            $('.js-singleselect').addClass('aspNetDisabled');
          }
          else {
            $('.js-singleselect').removeClass('aspNetDisabled');
          }
        });

        $createFolder.off('click').on('click', function () {
          $createFolderDiv.fadeToggle();
          $createFolderInput.val('');
        });

        $createFolderCancel.off('click').on('click', function () {
          $createFolderDiv.fadeOut();
          $createFolderInput.val('');
        });

        $renameFile.off('click').on('click', function () {
          $renameFileDiv.fadeToggle();
          $renameFileInput.val('');
        });

        $renameFileCancel.off('click').on('click', function () {
          $renameFileDiv.fadeOut();
          $renameFileInput.val('');
        });

        $deleteFolder.off('click').on('click', function (e) {
          if ($(this).attr('disabled') == 'disabled') {
            return false;
          }

          Rock.dialogs.confirmDelete(e, 'folder and all its contents');
        });
      }
    };

    return exports;

  }());
}(jQuery));



