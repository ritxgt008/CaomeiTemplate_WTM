﻿
<template>
  <BatchFormViewBatchEdit />

</template>


<script lang="ts">
import {Inject,mixins,Options,Provide,Vue } from "vue-property-decorator";
import {$locales} from "@/client";
import { ExFrameworkUserPageController,} from "../controller";
import BatchFormViewBatchEdit from "./widgets/batchFormViewBatchEdit.vue";
@Options({
    name: "Sys_BatchEdit",  
    components: {
    BatchFormViewBatchEdit,
  },
})
export default class extends Vue {

    readonly ExFrameworkUserPageController = ExFrameworkUserPageController;

    
    get queryKey() {
      return "adminframeworkuserbatchedit";
    }

    
    created() {}
    mounted() {}
}
</script>
<style lang="less">
</style>

